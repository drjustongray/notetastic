using System;
using System.Collections.Generic;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.AuthTokenTests
{
	public class AuthToken_Equals
	{
		private readonly List<AuthToken> _list1 = new List<AuthToken>();
		private readonly List<AuthToken> _list2 = new List<AuthToken>();

		public AuthToken_Equals()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var token in new string[] { null, "sometoken", "someothertoken" })
					foreach (var uid in new string[] { null, "uid1", "uid2" })
						foreach (var dateTime in new DateTimeOffset?[] { null, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(9) })
						{
							_list1.Add(new AuthToken
							{
								Id = id,
								Token = token,
								UID = uid,
								ExpiresAt = dateTime
							});
							_list2.Add(new AuthToken
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
			User user = new User();
			Assert.False(user.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var user in _list1)
			{
				Assert.True(user.Equals(user));
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