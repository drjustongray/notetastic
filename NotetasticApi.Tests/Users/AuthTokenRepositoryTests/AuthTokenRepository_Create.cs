using System;
using NotetasticApi.Common;
using NotetasticApi.Tests.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.AuthTokenRepositoryTests
{
	public class AuthTokenRepository_Create : AuthTokenRepository_Base
	{
		public AuthTokenRepository_Create(DatabaseFixture fixture) : base(fixture)
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
				() => _repo.Create(new AuthToken())
			);
			AssertCollectionEquals();
		}


		[Theory]
		[InlineData("null")]
		[InlineData("")]
		public async void ShouldThrowIfIdNonNull(string value)
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Create(new AuthToken { UID = "f@gd", Token = "value", Id = value, ExpiresAt = DateTimeOffset.Now })
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		public async void ShouldThrowIfTokenInUse(string token)
		{
			await Assert.ThrowsAsync<DocumentConflictException>(
				() => _repo.Create(new AuthToken { Token = token, UID = "uid3", ExpiresAt = DateTimeOffset.Now.AddMinutes(3) })
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("asdf", "uid3")]
		[InlineData("ad089708", "uid2")]
		public async void ShouldAddDocToDB(string token, string uid)
		{
			var authToken = new AuthToken { Token = token, UID = uid, ExpiresAt = DateTimeOffset.Now.AddMinutes(3) };
			await _repo.Create(authToken);

			_expectedTokens.Add(authToken);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("asdf", "uid3")]
		[InlineData("ad089708", "uid2")]
		public async void ShouldReturnUserDoc(string token, string uid)
		{
			var authToken = new AuthToken { Token = token, UID = uid, ExpiresAt = DateTimeOffset.Now.AddMinutes(3) };
			var anAuthToken = await _repo.Create(authToken);

			Assert.Equal(authToken, anAuthToken);
		}
	}
}