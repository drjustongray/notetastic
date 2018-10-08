using System;
using System.Linq;
using NotetasticApi.Common;
using NotetasticApi.Tests.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.AuthTokenRepositoryTests
{
	public class AuthTokenRepository_Update : AuthTokenRepository_Base
	{
		public AuthTokenRepository_Update(DatabaseFixture fixture) : base(fixture)
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
				() => _repo.Update(new AuthToken())
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ShouldThrowIfIdNull()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Update(new AuthToken
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
				() => _repo.Update(new AuthToken
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
		public async void ShouldThrowIfUIDMismatch(string tokenString, string uid)
		{
			var token = _expectedTokens.First(_ => _.Token == tokenString);
			token = new AuthToken
			{
				Id = token.Id,
				UID = uid,
				Token = token.Token + "asdf",
				ExpiresAt = token.ExpiresAt?.AddDays(3)
			};
			await Assert.ThrowsAsync<DocumentNotFoundException>(
				() => _repo.Update(token)
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
				() => _repo.Update(new AuthToken
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
			var token = _expectedTokens.First(_ => _.Token == from);
			_expectedTokens.Remove(token);
			token.Token = to;
			token.ExpiresAt += TimeSpan.FromDays(2);
			_expectedTokens.Add(token);

			await _repo.Update(token);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1", "token4")]
		[InlineData("token2", "token5")]
		[InlineData("token3", "token6")]
		public async void ShouldReturnUserDoc(string from, string to)
		{
			var token = _expectedTokens.First(_ => _.Token == from);
			token.Token = to;
			token.ExpiresAt += TimeSpan.FromDays(2);

			var aToken = await _repo.Update(token);

			Assert.Equal(token, aToken);
		}
	}
}