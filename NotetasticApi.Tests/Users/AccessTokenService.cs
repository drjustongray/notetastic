using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users
{
	public class AccessTokenService
	{
		private readonly SymmetricSecurityKey _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superdupersecret"));
		private readonly NotetasticApi.Users.AccessTokenService _service;

		public AccessTokenService()
		{
			_service = new NotetasticApi.Users.AccessTokenService(_key);
		}

		[Fact]
		public void ThrowsIfUIDNull()
		{
			Assert.Throws<ArgumentNullException>(
				() => _service.CreateAccessToken(null)
			);
		}

		[Theory]
		[InlineData("asdf")]
		[InlineData("3as3sd4")]
		[InlineData("asdf0988098")]
		public void ReturnsValidJWT(string uid)
		{
			SecurityToken validatedToken;
			var token = _service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				IssuerSigningKey = _key,
				ValidateAudience = false,
				ValidateIssuer = false
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
		}

		[Theory]
		[InlineData("asdf")]
		[InlineData("3as3sd4")]
		[InlineData("asdf0988098")]
		public void IncludesUidClaim(string uid)
		{
			SecurityToken validatedToken;
			var token = _service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				IssuerSigningKey = _key,
				ValidateAudience = false,
				ValidateIssuer = false
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
			var jwt = validatedToken as JwtSecurityToken;
			var uidClaim = jwt.Claims.Single(x => x.Type == ClaimTypes.UID);
			Assert.Equal(uid, uidClaim.Value);
		}

		[Theory]
		[InlineData("asdf")]
		[InlineData("3as3sd4")]
		[InlineData("asdf0988098")]
		public void ExpiresInApprox5Minutes(string uid)
		{
			SecurityToken validatedToken;
			var token = _service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				IssuerSigningKey = _key,
				ValidateAudience = false,
				ValidateIssuer = false
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
			Assert.InRange<DateTime>(validatedToken.ValidTo, DateTime.UtcNow.AddMinutes(4.9), DateTime.UtcNow.AddMinutes(5.1));
		}

		[Theory]
		[InlineData("asdf")]
		[InlineData("3as3sd4")]
		[InlineData("asdf0988098")]
		public void NotValidBefore(string uid)
		{
			var now = DateTime.UtcNow.AddSeconds(-1);
			SecurityToken validatedToken;
			var token = _service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				IssuerSigningKey = _key,
				ValidateAudience = false,
				ValidateIssuer = false
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
			Assert.InRange<DateTime>(validatedToken.ValidFrom, now, DateTime.UtcNow.AddSeconds(1));
		}
	}
}