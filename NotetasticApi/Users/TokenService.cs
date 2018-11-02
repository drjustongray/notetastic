using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using NotetasticApi.Common;

namespace NotetasticApi.Users
{

	public static class ClaimTypes
	{
		public const string UID = "UID";
	}

	public interface ITokenService
	{
		string CreateAccessToken(string uid);
		string CreateRefreshToken();
	}

	public class TokenService : ITokenService
	{
		private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
		private readonly SecurityKey key;
		private readonly string audience;
		private readonly string issuer;
		private readonly ITimeService timeService;
		private readonly SigningCredentials credentials;
		private RandomNumberGenerator rng = RandomNumberGenerator.Create();

		public TokenService(SecurityKey key, string audience, string issuer, ITimeService timeService)
		{
			this.key = key;
			this.audience = audience;
			this.issuer = issuer;
			this.timeService = timeService;
			credentials = new SigningCredentials(this.key, SecurityAlgorithms.HmacSha256);
		}

		public string CreateAccessToken(string uid)
		{
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}
			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: new Claim[] { new Claim(ClaimTypes.UID, uid) },
				signingCredentials: credentials,
				notBefore: timeService.GetCurrentTime().UtcDateTime,
				expires: timeService.GetCurrentTime().AddMinutes(5).UtcDateTime
			);
			return tokenHandler.WriteToken(token);
		}

		public string CreateRefreshToken()
		{
			var randomNumber = new byte[32];
			rng.GetBytes(randomNumber);
			var token = Convert.ToBase64String(randomNumber);
			return token;
		}
	}
}