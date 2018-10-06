using System.Collections.Generic;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRecord
{
	public class User_EqualityOperators
	{
		private readonly List<User> _list1 = new List<User>();
		private readonly List<User> _list2 = new List<User>();

		public User_EqualityOperators()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
			{
				foreach (var username in new string[] { null, "someusername", "someotherusername" })
				{
					foreach (var hash in new string[] { null, "hash1", "hash2" })
					{
						_list1.Add(new User
						{
							Id = id,
							UserName = username,
							PasswordHash = hash
						});
						_list2.Add(new User
						{
							Id = id,
							UserName = username,
							PasswordHash = hash
						});
					}
				}
			}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			User user = new User();
			Assert.False(user == null);
			Assert.False(null == user);
			Assert.True(user != null);
			Assert.True(null != user);
			user = null;
			Assert.True(user == null);
			Assert.True(null == user);
			Assert.False(user != null);
			Assert.False(null != user);
		}

		[Fact]
		public void AgreesWithEquals()
		{
			for (int i = 0; i < _list1.Count; i++)
				for (int j = 0; j < _list1.Count; j++)
				{
					var u1 = _list1[i];
					var u2 = _list2[j];
					Assert.Equal(u1.Equals(u2), u1 == u2);
					Assert.Equal(!u1.Equals(u2), u1 != u2);
				}
		}
	}
}