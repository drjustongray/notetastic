using System.Collections.Generic;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.TokenPairTests
{
	public class TokenPair_operators
	{
		private readonly List<TokenPair> list1 = new List<TokenPair>();
		private readonly List<TokenPair> list2 = new List<TokenPair>();

		public TokenPair_operators()
		{
			foreach (var accessToken in new string[] { null, "ax1", "ax2" })
				foreach (var refreshToken in new string[] { null, "ref1", "ref2" })
				{
					list1.Add(new TokenPair
					{
						AccessToken = accessToken,
						RefreshToken = refreshToken
					});
					list2.Add(new TokenPair
					{
						AccessToken = accessToken,
						RefreshToken = refreshToken
					});
				}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var tokenPair = new TokenPair();
			Assert.False(tokenPair == null);
			Assert.False(null == tokenPair);
			Assert.True(tokenPair != null);
			Assert.True(null != tokenPair);
			tokenPair = null;
			Assert.True(tokenPair == null);
			Assert.True(null == tokenPair);
			Assert.False(tokenPair != null);
			Assert.False(null != tokenPair);
		}

		[Fact]
		public void AgreesWithEquals()
		{
			for (int i = 0; i < list1.Count; i++)
				for (int j = 0; j < list1.Count; j++)
				{
					var u1 = list1[i];
					var u2 = list2[j];
					Assert.Equal(u1.Equals(u2), u1 == u2);
					Assert.Equal(!u1.Equals(u2), u1 != u2);
				}
		}
	}
}