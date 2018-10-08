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
		public async void DeletesToken(string tokenString)
		{
			var token = _expectedTokens.First(_ => _.Token == tokenString);
			_expectedTokens.Remove(token);
			await _repo.Delete(tokenString);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("token4")]
		[InlineData("token5")]
		[InlineData("token6")]
		public async void DoesNothingIfTokenDoesNotExist(string tokenString)
		{
			await _repo.Delete(tokenString);
			AssertCollectionEquals();
		}
	}
}