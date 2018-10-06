using System;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.AuthTokenTests
{
	public class AuthToken_IsValid
	{
		[Theory]
		[InlineData(null)]
		[InlineData("    ")]
		[InlineData("\n\t")]
		public void IsFalseIfUIDInvalid(string uid)
		{
			var authToken = new AuthToken
			{
				UID = uid,
				Token = "somehash",
				ExpiresAt = DateTimeOffset.Now
			};
			Assert.False(authToken.IsValid);
			authToken.Id = "asom";
			Assert.False(authToken.IsValid);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("    ")]
		[InlineData("\n\t")]
		public void IsFalseIfTokenInvalid(string token)
		{
			var authToken = new AuthToken
			{
				UID = "uid",
				Token = token,
				ExpiresAt = DateTimeOffset.Now
			};
			Assert.False(authToken.IsValid);
			authToken.Id = "asom";
			Assert.False(authToken.IsValid);
		}

		[Fact]
		public void IsFalseIfExpiresAtMissing()
		{
			var authToken = new AuthToken
			{
				UID = "uid",
				Token = "token"
			};
			Assert.False(authToken.IsValid);
			authToken.Id = "asom";
			Assert.False(authToken.IsValid);
		}

		[Fact]
		public void IsTrueIfAllThingsPresent()
		{
			var authToken = new AuthToken
			{
				UID = "uid",
				Token = "token",
				ExpiresAt = DateTimeOffset.Now
			};
			Assert.True(authToken.IsValid);
			authToken.Id = "asom";
			Assert.True(authToken.IsValid);
		}
	}
}