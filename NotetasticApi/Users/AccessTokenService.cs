using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NotetasticApi.Users
{

	public static class ClaimTypes
	{
		public const string UID = "UID";
	}

	public interface IAccessTokenService
	{
		string CreateAccessToken(string uid);
	}

	public class AccessTokenService : IAccessTokenService
	{
		private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
		private readonly SecurityKey _key;
		public AccessTokenService(SecurityKey key)
		{
			_key = key;
		}

		public string CreateAccessToken(string uid)
		{
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}
			var token = new JwtSecurityToken(
				claims: new Claim[] { new Claim(ClaimTypes.UID, uid) },
				signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256),
				notBefore: DateTime.Now,
				expires: DateTime.Now.AddMinutes(5)
			);
			return _tokenHandler.WriteToken(token);
		}
	}
}