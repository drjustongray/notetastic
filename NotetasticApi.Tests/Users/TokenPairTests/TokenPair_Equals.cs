using System.Collections.Generic;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.TokenPairTests
{
	public class TokenPair_Equals
	{
		private readonly List<TokenPair> list1 = new List<TokenPair>();
		private readonly List<TokenPair> list2 = new List<TokenPair>();

		public TokenPair_Equals()
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
			Assert.False(tokenPair.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var tokenPair in list1)
			{
				Assert.True(tokenPair.Equals(tokenPair));
			}
		}

		[Fact]
		public void ReturnsTrueIfIdentical()
		{
			for (int i = 0; i < list1.Count; i++)
			{
				Assert.True(list1[i].Equals(list2[i]));
			}
		}

		[Fact]
		public void ReturnsFalseIfDifferent()
		{
			for (int i = 0; i < list1.Count; i++)
				for (int j = 0; j < list1.Count; j++)
					if (i != j)
						Assert.False(list1[i].Equals(list2[j]));
		}
	}
}