using System;
using System.Linq;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenRepositoryTests
{
	public class RefreshTokenRepository_DeleteAll : RefreshTokenRepository_Base
	{
		public RefreshTokenRepository_DeleteAll(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfArgumentNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.DeleteAll(null)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void DeletesTokens(string uid)
		{
			var refreshTokens = _expectedTokens.Where(_ => _.UID == uid).ToHashSet();
			_expectedTokens.ExceptWith(refreshTokens);
			await _repo.DeleteAll(uid);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid3")]
		[InlineData("uid4")]
		[InlineData("uid5")]
		public async void DoesNothingIfTokensDoNotExist(string uid)
		{
			await _repo.DeleteAll(uid);
			AssertCollectionEquals();
		}
	}
}