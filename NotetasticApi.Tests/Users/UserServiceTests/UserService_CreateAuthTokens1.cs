using System;
using Moq;
using NotetasticApi.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_CreateAuthTokens1 : UserService_Base
	{
		public UserService_CreateAuthTokens1()
		{
			validationService.Setup(x => x.AssertNonNull<User>(It.IsNotNull<User>(), "user"));
			validationService.Setup(x => x.AssertNonNull<string>(It.IsNotNull<string>(), "Id"));
		}

		[Fact]
		public async void ValidatesUser()
		{
			var expected = new ArgumentNullException("user");
			validationService.Setup(x => x.AssertNonNull<User>(null, "user"))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentNullException>(
				() => userService.CreateAuthTokens((User)null, false)
			);
			Assert.Same(expected, actual);
		}

		[Fact]
		public async void ValidatesUserId()
		{
			var expected = new ArgumentNullException("Id");
			validationService.Setup(x => x.AssertNonNull<string>(null, "Id"))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentNullException>(
				() => userService.CreateAuthTokens(new User(), false)
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData("uid1", true)]
		[InlineData("uid2", false)]
		[InlineData("uid3", true)]
		public async void SavesRefreshTokenReturnsTokenPair(string uid, bool shouldPersist)
		{
			var user = new User { Id = uid, UserName = "someusername" };
			var expected = new TokenPair
			{
				AccessToken = "axessToken" + uid,
				RefreshToken = "refreshToken" + uid,
				User = user,
				Persistent = shouldPersist
			};
			var refreshToken = new RefreshToken
			{
				Token = expected.RefreshToken,
				UID = uid,
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			tokenService.Setup(x => x.CreateAccessToken(uid)).Returns(expected.AccessToken);
			tokenService.Setup(x => x.CreateRefreshToken()).Returns(expected.RefreshToken);
			refreshTokenRepo.Setup(x => x.Create(Matches(refreshToken))).ReturnsAsync(refreshToken);

			var actual = await userService.CreateAuthTokens(user, shouldPersist);
			Assert.Equal(expected, actual);
			refreshTokenRepo.Verify(x => x.Create(It.IsAny<RefreshToken>()), Times.Once);
		}

		[Theory]
		[InlineData("uid1", false)]
		[InlineData("uid2", true)]
		[InlineData("uid3", false)]
		public async void RetriesOnDocumentConflictError(string uid, bool shouldPersist)
		{
			var user = new User { Id = uid, UserName = "someusername" };
			var expected = new TokenPair
			{
				AccessToken = "axessToken" + uid,
				RefreshToken = "refreshToken" + uid,
				User = user,
				Persistent = shouldPersist
			};
			var refreshToken = new RefreshToken
			{
				Token = expected.RefreshToken,
				UID = uid,
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};

			var duplicateToken = new RefreshToken
			{
				UID = uid,
				Token = "tokenthatsalreadyinuse",
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			tokenService.Setup(x => x.CreateAccessToken(uid)).Returns(expected.AccessToken);
			tokenService.SetupSequence(x => x.CreateRefreshToken())
				.Returns(duplicateToken.Token)
				.Returns(expected.RefreshToken);
			refreshTokenRepo.SetupSequence(x => x.Create(Matches(duplicateToken)))
				.ThrowsAsync(new DocumentConflictException());
			refreshTokenRepo.SetupSequence(x => x.Create(Matches(refreshToken)))
				.ReturnsAsync(refreshToken);

			var actual = await userService.CreateAuthTokens(user, shouldPersist);
			Assert.Equal(expected, actual);
			refreshTokenRepo.Verify(x => x.Create(It.IsNotNull<RefreshToken>()), Times.Exactly(2));
		}

		[Theory]
		[InlineData("uid1", true)]
		[InlineData("uid2", false)]
		[InlineData("uid3", true)]
		public async void RetriesOnceOnDocumentConflictError(string uid, bool shouldPersist)
		{
			var duplicateToken1 = new RefreshToken
			{
				UID = uid,
				Token = "tokenthatsalreadyinuse",
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			var duplicateToken2 = new RefreshToken
			{
				UID = uid,
				Token = "anothertokenthatsalreadyinuse",
				ExpiresAt = DateTimeOffset.Now.AddDays(30),
				Persistent = shouldPersist
			};
			tokenService.SetupSequence(x => x.CreateRefreshToken())
				.Returns(duplicateToken1.Token)
				.Returns(duplicateToken2.Token);
			refreshTokenRepo.SetupSequence(x => x.Create(Matches(duplicateToken1)))
				.ThrowsAsync(new DocumentConflictException());
			refreshTokenRepo.SetupSequence(x => x.Create(Matches(duplicateToken2)))
				.ThrowsAsync(new DocumentConflictException());

			await Assert.ThrowsAsync<DocumentConflictException>(
				() => userService.CreateAuthTokens(new User { Id = uid }, shouldPersist)
			);

			refreshTokenRepo.Verify(x => x.Create(It.IsNotNull<RefreshToken>()), Times.Exactly(2));
		}
	}
}