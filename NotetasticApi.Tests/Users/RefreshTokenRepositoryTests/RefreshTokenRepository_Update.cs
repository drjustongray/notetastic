using System;
using System.Linq;
using NotetasticApi.Common;
using NotetasticApi.Tests.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenRepositoryTests
{
	public class RefreshTokenRepository_Update : RefreshTokenRepository_Base
	{
		public RefreshTokenRepository_Update(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfArgNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.Update(null)
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ShouldThrowIfTokenInvalid()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Update(new RefreshToken())
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ShouldThrowIfIdNull()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Update(new RefreshToken
				{
					UID = "f@gd",
					Token = "value",
					ExpiresAt = DateTimeOffset.Now
				})
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("id1")]
		[InlineData("id2")]
		public async void ShouldThrowIfDocumentMissing(string id)
		{
			await Assert.ThrowsAsync<DocumentNotFoundException>(
				() => _repo.Update(new RefreshToken
				{
					Id = id,
					UID = "whatevs",
					Token = "somesuch",
					ExpiresAt = DateTimeOffset.Now.AddDays(3)
				})
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1", "uid2")]
		[InlineData("token2", "uid3")]
		[InlineData("token3", "uid1")]
		public async void ShouldThrowIfUIDMismatch(string token, string uid)
		{
			var refreshToken = _expectedTokens.First(_ => _.Token == token);
			refreshToken = new RefreshToken
			{
				Id = refreshToken.Id,
				UID = uid,
				Token = refreshToken.Token + "asdf",
				ExpiresAt = refreshToken.ExpiresAt?.AddDays(3)
			};
			await Assert.ThrowsAsync<DocumentNotFoundException>(
				() => _repo.Update(refreshToken)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1", "token2")]
		[InlineData("token2", "token3")]
		[InlineData("token3", "token1")]
		public async void ShouldThrowIfTokenInUse(string token, string other)
		{
			var toReplace = _expectedTokens.First(_ => _.Token == token);
			await Assert.ThrowsAsync<DocumentConflictException>(
				() => _repo.Update(new RefreshToken
				{
					Id = toReplace.Id,
					Token = other,
					UID = toReplace.UID,
					ExpiresAt = DateTimeOffset.Now.AddMinutes(3)
				})
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1", "token4")]
		[InlineData("token2", "token5")]
		[InlineData("token3", "token6")]
		public async void ShouldUpdateDocInDB(string from, string to)
		{
			var refreshToken = _expectedTokens.First(_ => _.Token == from);
			_expectedTokens.Remove(refreshToken);
			refreshToken.Token = to;
			refreshToken.ExpiresAt += TimeSpan.FromDays(2);
			_expectedTokens.Add(refreshToken);

			await _repo.Update(refreshToken);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1", "token4")]
		[InlineData("token2", "token5")]
		[InlineData("token3", "token6")]
		public async void ShouldReturnUserDoc(string from, string to)
		{
			var expected = _expectedTokens.First(_ => _.Token == from);
			expected.Token = to;
			expected.ExpiresAt += TimeSpan.FromDays(2);

			var actual = await _repo.Update(expected);

			Assert.Equal(expected, actual);
		}
	}
}