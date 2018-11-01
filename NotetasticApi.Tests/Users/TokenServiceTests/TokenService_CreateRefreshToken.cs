using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace NotetasticApi.Tests.Users.TokenServiceTests
{
	public class TokenService_CreateRefreshToken
	{
		[Fact]
		public void ReturnsRandomStrings()
		{
			var service = new NotetasticApi.Users.TokenService(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superdupersecret")),
				"audience",
				"issuer"
			);

			HashSet<string> tokens = new HashSet<string>();
			for (int i = 0; i < 1000; i++)
			{
				var token = service.CreateRefreshToken();
				Assert.True(token.Length >= 32);
				tokens.Add(token);
			}
			Assert.Equal(1000, tokens.Count);
		}
	}
}