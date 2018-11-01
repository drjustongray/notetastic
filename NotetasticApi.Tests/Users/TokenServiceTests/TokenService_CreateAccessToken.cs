using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.TokenServiceTests
{
	public class TokenService_CreateAccessToken
	{

		[Fact]
		public void ThrowsIfUIDNull()
		{
			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key"));
			var service = new TokenService(signingKey, "audience", "issuer");
			Assert.Throws<ArgumentNullException>(
				() => service.CreateAccessToken(null)
			);
		}

		[Theory]
		[InlineData("asdf", "suberdubersecret", "service1", "service2")]
		[InlineData("3as3sd4", "anothersecretforfun", "hello1", "try2")]
		[InlineData("asdf0988098", "fasdklfsdf89070987098", "asldkfjds", "afsdfdr")]
		public void ReturnsValidJWT(string uid, string key, string issuer, string audience)
		{
			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			var service = new TokenService(signingKey, audience, issuer);
			SecurityToken validatedToken;
			var token = service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				ValidAudience = audience,
				ValidIssuer = issuer,
				IssuerSigningKey = signingKey
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
		}

		[Theory]
		[InlineData("asdf", "suberdubersecret", "service1", "service2")]
		[InlineData("3as3sd4", "anothersecretforfun", "hello1", "try2")]
		[InlineData("asdf0988098", "fasdklfsdf89070987098", "asldkfjds", "afsdfdr")]
		public void IncludesUidClaim(string uid, string key, string issuer, string audience)
		{
			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			var service = new TokenService(signingKey, audience, issuer);
			SecurityToken validatedToken;
			var token = service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				ValidAudience = audience,
				ValidIssuer = issuer,
				IssuerSigningKey = signingKey
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
			var jwt = validatedToken as JwtSecurityToken;
			var uidClaim = jwt.Claims.Single(x => x.Type == ClaimTypes.UID);
			Assert.Equal(uid, uidClaim.Value);
		}

		[Theory]
		[InlineData("asdf", "suberdubersecret", "service1", "service2")]
		[InlineData("3as3sd4", "anothersecretforfun", "hello1", "try2")]
		[InlineData("asdf0988098", "fasdklfsdf89070987098", "asldkfjds", "afsdfdr")]
		public void ExpiresInApprox5Minutes(string uid, string key, string issuer, string audience)
		{
			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			var service = new TokenService(signingKey, audience, issuer);
			SecurityToken validatedToken;
			var token = service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				ValidAudience = audience,
				ValidIssuer = issuer,
				IssuerSigningKey = signingKey
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
			Assert.InRange<DateTime>(validatedToken.ValidTo, DateTime.UtcNow.AddMinutes(4.9), DateTime.UtcNow.AddMinutes(5.1));
		}

		[Theory]
		[InlineData("asdf", "suberdubersecret", "service1", "service2")]
		[InlineData("3as3sd4", "anothersecretforfun", "hello1", "try2")]
		[InlineData("asdf0988098", "fasdklfsdf89070987098", "asldkfjds", "afsdfdr")]
		public void NotValidBefore(string uid, string key, string issuer, string audience)
		{
			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			var service = new TokenService(signingKey, audience, issuer);
			var now = DateTime.UtcNow.AddSeconds(-1);
			SecurityToken validatedToken;
			var token = service.CreateAccessToken(uid);
			var validationParams = new TokenValidationParameters()
			{
				ValidAudience = audience,
				ValidIssuer = issuer,
				IssuerSigningKey = signingKey
			};
			new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out validatedToken);
			Assert.InRange<DateTime>(validatedToken.ValidFrom, now, DateTime.UtcNow.AddSeconds(1));
		}
	}
}