using System;
using System.Linq;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Users.AuthTokenRepositoryTests
{
	public class AuthTokenRepository_Find : AuthTokenRepository_Base
	{
		public AuthTokenRepository_Find(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfArgumentNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.Find(null)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		public async void ReturnsAuthTokenIfFound(string token)
		{
			var authToken = await _repo.Find(token);
			Assert.Equal(authToken, _expectedTokens.First(_ => _.Token == token));
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsNullIfAuthTokenNotFound()
		{
			var token = await _repo.Find("asdfa");
			Assert.Null(token);
			AssertCollectionEquals();
		}
	}
}