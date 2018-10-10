using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

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
		private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
		private readonly SecurityKey _key;
		private readonly SigningCredentials _credentials;
		private RandomNumberGenerator rng = RandomNumberGenerator.Create();
		public TokenService(SecurityKey key)
		{
			_key = key;
			_credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
		}

		public string CreateAccessToken(string uid)
		{
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}
			var token = new JwtSecurityToken(
				claims: new Claim[] { new Claim(ClaimTypes.UID, uid) },
				signingCredentials: _credentials,
				notBefore: DateTime.Now,
				expires: DateTime.Now.AddMinutes(5)
			);
			return _tokenHandler.WriteToken(token);
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