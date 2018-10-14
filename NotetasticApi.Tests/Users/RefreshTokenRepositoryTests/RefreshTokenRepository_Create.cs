using System;
using NotetasticApi.Common;
using NotetasticApi.Tests.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenRepositoryTests
{
	public class RefreshTokenRepository_Create : RefreshTokenRepository_Base
	{
		public RefreshTokenRepository_Create(DatabaseFixture fixture) : base(fixture)
		{
		}


		[Fact]
		public async void ShouldThrowIfArgNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.Create(null)
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ShouldThrowIfTokenInvalid()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Create(new RefreshToken())
			);
			AssertCollectionEquals();
		}


		[Theory]
		[InlineData("null")]
		[InlineData("")]
		public async void ShouldThrowIfIdNonNull(string value)
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Create(new RefreshToken
				{
					UID = "f@gd",
					Token = "value",
					Id = value,
					ExpiresAt = DateTimeOffset.Now
				})
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		public async void ShouldThrowIfTokenInUse(string token)
		{
			await Assert.ThrowsAsync<DocumentConflictException>(
				() => _repo.Create(new RefreshToken
				{
					Token = token,
					UID = "uid3",
					ExpiresAt = DateTimeOffset.Now.AddMinutes(3)
				})
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("asdf", "uid3", true)]
		[InlineData("ad089708", "uid2", false)]
		public async void ShouldAddDocToDB(string token, string uid, bool persistent)
		{
			var refreshToken = new RefreshToken
			{
				Token = token,
				UID = uid,
				ExpiresAt = DateTimeOffset.Now.AddMinutes(3),
				Persistent = persistent
			};
			await _repo.Create(refreshToken);

			_expectedTokens.Add(refreshToken);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("asdf", "uid3")]
		[InlineData("ad089708", "uid2")]
		public async void ShouldReturnUserDoc(string token, string uid)
		{
			var expected = new RefreshToken
			{
				Token = token,
				UID = uid,
				ExpiresAt = DateTimeOffset.Now.AddMinutes(3)
			};
			var actual = await _repo.Create(expected);

			Assert.Equal(expected, actual);
		}
	}
}