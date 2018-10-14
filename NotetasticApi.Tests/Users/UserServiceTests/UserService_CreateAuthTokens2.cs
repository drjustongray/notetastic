using System;
using Moq;
using NotetasticApi.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_CreateAuthTokens2 : UserService_Base
	{
		public UserService_CreateAuthTokens2()
		{
			validationService.Setup(x => x.AssertNonNull<string>(It.IsNotNull<string>(), "refreshToken"));
		}

		[Fact]
		public async void ValidatesToken()
		{
			var expected = new ArgumentNullException("refreshToken");
			validationService.Setup(x => x.AssertNonNull<string>(null, "refreshToken"))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentNullException>(
				() => userService.CreateAuthTokens((string)null)
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData("reftok1")]
		[InlineData("reftok2")]
		[InlineData("reftok3")]
		public async void ReturnsNullIfTokenNotFound(string token)
		{
			refreshTokenRepo.Setup(x => x.Find(token)).ReturnsAsync((RefreshToken)null);
			var result = await userService.CreateAuthTokens(token);
			Assert.Null(result);
		}

		[Theory]
		[InlineData("reftok1", true)]
		[InlineData("reftok2", false)]
		[InlineData("reftok3", true)]
		public async void ReturnsNullIfTokenExpired(string token, bool shouldPersist)
		{
			var tokenDoc = new RefreshToken
			{
				Id = "someid" + token,
				Token = token,
				UID = "uid",
				ExpiresAt = DateTimeOffset.Now.AddDays(-2),
				Persistent = shouldPersist
			};
			refreshTokenRepo.Setup(x => x.Find(token)).ReturnsAsync(tokenDoc);
			var result = await userService.CreateAuthTokens(token);
			Assert.Null(result);
		}

		[Theory]
		[InlineData("reftok1", "uid1", false)]
		[InlineData("reftok2", "uid2", true)]
		[InlineData("reftok3", "uid3", false)]
		public async void UpdatesRefreshTokenReturnsTokenPair(string token, string uid, bool shouldPersist)
		{
			var user = new User { Id = uid, UserName = "someusername" };
			userRepo.Setup(x => x.FindById(uid)).ReturnsAsync(user);

			var tokenDoc = new RefreshToken
			{
				Id = "someid" + token,
				Token = token,
				UID = uid,
				ExpiresAt = DateTimeOffset.Now.AddDays(23),
				Persistent = shouldPersist
			};
			refreshTokenRepo.Setup(x => x.Find(token)).ReturnsAsync(tokenDoc);

			var newToken = new RefreshToken
			{
				Id = tokenDoc.Id,
				UID = uid,
				Token = "newtoken" + token,
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			refreshTokenRepo.SetupSequence(x => x.Update(Matches(newToken))).ReturnsAsync(newToken);
			var accessToken = uid + "token";
			tokenService.Setup(x => x.CreateAccessToken(uid)).Returns(accessToken);
			tokenService.Setup(x => x.CreateRefreshToken()).Returns(newToken.Token);

			var result = await userService.CreateAuthTokens(token);
			refreshTokenRepo.Verify(x => x.Update(It.IsAny<RefreshToken>()));
			Assert.Equal(new TokenPair
			{
				RefreshToken = newToken.Token,
				AccessToken = accessToken,
				User = user,
				Persistent = shouldPersist
			}, result);
		}

		[Theory]
		[InlineData("reftok1", "uid1", true)]
		[InlineData("reftok2", "uid2", false)]
		[InlineData("reftok3", "uid3", true)]
		public async void RetriesOnDocumentConflictError(string token, string uid, bool shouldPersist)
		{
			var user = new User { Id = uid, UserName = "someusername" };
			userRepo.Setup(x => x.FindById(uid)).ReturnsAsync(user);

			var tokenDoc = new RefreshToken
			{
				Id = "someid" + token,
				Token = token,
				UID = uid,
				ExpiresAt = DateTimeOffset.Now.AddDays(23),
				Persistent = shouldPersist
			};
			refreshTokenRepo.Setup(x => x.Find(token)).ReturnsAsync(tokenDoc);

			var duplicateToken = new RefreshToken
			{
				Id = tokenDoc.Id,
				UID = uid,
				Token = "tokenthatsalreadyinuse",
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			var refreshToken = new RefreshToken
			{
				Id = tokenDoc.Id,
				UID = uid,
				Token = "newtoken" + token,
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			refreshTokenRepo.SetupSequence(x => x.Update(Matches(refreshToken)))
				.ReturnsAsync(refreshToken);
			refreshTokenRepo.SetupSequence(x => x.Update(Matches(duplicateToken)))
				.ThrowsAsync(new DocumentConflictException());

			var accessToken = uid + "token";
			tokenService.Setup(x => x.CreateAccessToken(uid)).Returns(accessToken);
			tokenService.SetupSequence(x => x.CreateRefreshToken())
				.Returns(duplicateToken.Token)
				.Returns(refreshToken.Token);

			var result = await userService.CreateAuthTokens(token);
			refreshTokenRepo.Verify(x => x.Update(It.IsNotNull<RefreshToken>()), Times.Exactly(2));
			Assert.Equal(new TokenPair
			{
				RefreshToken = refreshToken.Token,
				AccessToken = accessToken,
				User = user,
				Persistent = shouldPersist
			}, result);
		}

		[Theory]
		[InlineData("reftok1", "uid1", false)]
		[InlineData("reftok2", "uid2", true)]
		[InlineData("reftok3", "uid3", false)]
		public async void RetriesOnceOnDocumentConflictError(string token, string uid, bool shouldPersist)
		{
			var tokenDoc = new RefreshToken
			{
				Id = "someid" + token,
				Token = token,
				UID = uid,
				ExpiresAt = DateTimeOffset.Now.AddDays(23),
				Persistent = shouldPersist
			};
			refreshTokenRepo.Setup(x => x.Find(token)).ReturnsAsync(tokenDoc);

			var duplicateToken1 = new RefreshToken
			{
				Id = tokenDoc.Id,
				UID = uid,
				Token = "tokenthatsalreadyinuse",
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			var duplicateToken2 = new RefreshToken
			{
				Id = tokenDoc.Id,
				UID = uid,
				Token = "anothertokenthatsalreadyinuse",
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			tokenService.SetupSequence(x => x.CreateRefreshToken())
				.Returns(duplicateToken1.Token)
				.Returns(duplicateToken2.Token);
			refreshTokenRepo.SetupSequence(x => x.Update(Matches(duplicateToken1)))
				.ThrowsAsync(new DocumentConflictException());
			refreshTokenRepo.SetupSequence(x => x.Update(Matches(duplicateToken2)))
				.ThrowsAsync(new DocumentConflictException());

			await Assert.ThrowsAsync<DocumentConflictException>(
				() => userService.CreateAuthTokens(token)
			);

			refreshTokenRepo.Verify(x => x.Update(It.IsNotNull<RefreshToken>()), Times.Exactly(2));
		}
	}
}