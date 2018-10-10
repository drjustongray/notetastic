using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace NotetasticApi.Tests.Users.TokenServiceTests
{
	public class TokenService_CreateRefreshToken
	{
		private readonly NotetasticApi.Users.TokenService _service;

		public TokenService_CreateRefreshToken()
		{
			_service = new NotetasticApi.Users.TokenService(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superdupersecret"))
			);
		}
		[Fact]
		public void ReturnsRandomStrings()
		{
			HashSet<string> tokens = new HashSet<string>();
			for (int i = 0; i < 1000; i++)
			{
				var token = _service.CreateRefreshToken();
				Assert.True(token.Length >= 32);
				tokens.Add(token);
			}
			Assert.Equal(1000, tokens.Count);
		}
	}
}