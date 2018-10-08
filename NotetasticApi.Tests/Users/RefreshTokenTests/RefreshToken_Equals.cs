using System;
using System.Collections.Generic;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenTests
{
	public class RefreshToken_Equals
	{
		private readonly List<RefreshToken> _list1 = new List<RefreshToken>();
		private readonly List<RefreshToken> _list2 = new List<RefreshToken>();

		public RefreshToken_Equals()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var token in new string[] { null, "sometoken", "someothertoken" })
					foreach (var uid in new string[] { null, "uid1", "uid2" })
						foreach (var dateTime in new DateTimeOffset?[] { null, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(9) })
						{
							_list1.Add(new RefreshToken
							{
								Id = id,
								Token = token,
								UID = uid,
								ExpiresAt = dateTime
							});
							_list2.Add(new RefreshToken
							{
								Id = id,
								Token = token,
								UID = uid,
								ExpiresAt = dateTime
							});
						}

		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			RefreshToken refreshToken = new RefreshToken();
			Assert.False(refreshToken.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var refreshToken in _list1)
			{
				Assert.True(refreshToken.Equals(refreshToken));
			}
		}

		[Fact]
		public void ReturnsTrueIfIdentical()
		{
			for (int i = 0; i < _list1.Count; i++)
			{
				Assert.True(_list1[i].Equals(_list2[i]));
			}
		}

		[Fact]
		public void ReturnsFalseIfDifferent()
		{
			for (int i = 0; i < _list1.Count; i++)
				for (int j = 0; j < _list1.Count; j++)
					if (i != j)
						Assert.False(_list1[i].Equals(_list2[j]));
		}
	}
}