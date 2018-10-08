using System;
using System.Linq;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenRepositoryTests
{
	public class RefreshTokenRepository_Delete : RefreshTokenRepository_Base
	{
		public RefreshTokenRepository_Delete(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfArgumentNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.Delete(null)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void DeletesToken(string token)
		{
			var refreshToken = _expectedTokens.First(_ => _.Token == token);
			_expectedTokens.Remove(refreshToken);
			await _repo.Delete(token);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token4")]
		[InlineData("token5")]
		[InlineData("token6")]
		public async void DoesNothingIfTokenDoesNotExist(string token)
		{
			await _repo.Delete(token);
			AssertCollectionEquals();
		}
	}
}