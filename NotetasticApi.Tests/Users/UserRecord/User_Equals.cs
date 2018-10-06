using System.Collections.Generic;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRecord
{
	public class User_Equals
	{

		private readonly List<User> _list1 = new List<User>();
		private readonly List<User> _list2 = new List<User>();

		public User_Equals()
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