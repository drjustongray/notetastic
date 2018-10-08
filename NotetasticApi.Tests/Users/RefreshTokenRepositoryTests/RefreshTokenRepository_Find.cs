using System;
using System.Linq;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenRepositoryTests
{
	public class RefreshTokenRepository_Find : RefreshTokenRepository_Base
	{
		public RefreshTokenRepository_Find(DatabaseFixture fixture) : base(fixture)
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
			var refreshToken = await _repo.Find(token);
			Assert.Equal(refreshToken, _expectedTokens.First(_ => _.Token == token));
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsNullIfAuthTokenNotFound()
		{
			var refreshToken = await _repo.Find("asdfa");
			Assert.Null(refreshToken);
			AssertCollectionEquals();
		}
	}
}