using System;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenTests
{
	public class RefreshToken_IsValid
	{
		[Theory]
		[InlineData(null)]
		[InlineData("    ")]
		[InlineData("\n\t")]
		public void IsFalseIfUIDInvalid(string uid)
		{
			var refreshToken = new RefreshToken
			{
				UID = uid,
				Token = "somehash",
				ExpiresAt = DateTimeOffset.Now
			};
			Assert.False(refreshToken.IsValid);
			refreshToken.Id = "asom";
			Assert.False(refreshToken.IsValid);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("    ")]
		[InlineData("\n\t")]
		public void IsFalseIfTokenInvalid(string token)
		{
			var refreshToken = new RefreshToken
			{
				UID = "uid",
				Token = token,
				ExpiresAt = DateTimeOffset.Now
			};
			Assert.False(refreshToken.IsValid);
			refreshToken.Id = "asom";
			Assert.False(refreshToken.IsValid);
		}

		[Fact]
		public void IsFalseIfExpiresAtMissing()
		{
			var refreshToken = new RefreshToken
			{
				UID = "uid",
				Token = "token"
			};
			Assert.False(refreshToken.IsValid);
			refreshToken.Id = "asom";
			Assert.False(refreshToken.IsValid);
		}

		[Fact]
		public void IsTrueIfAllThingsPresent()
		{
			var refreshToken = new RefreshToken
			{
				UID = "uid",
				Token = "token",
				ExpiresAt = DateTimeOffset.Now
			};
			Assert.True(refreshToken.IsValid);
			refreshToken.Id = "asom";
			Assert.True(refreshToken.IsValid);
		}
	}
}