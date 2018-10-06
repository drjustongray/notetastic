using System;
using System.Collections.Generic;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.AuthTokenTests
{
	public class AuthToken_EqualityOperators
	{
		private readonly List<AuthToken> _list1 = new List<AuthToken>();
		private readonly List<AuthToken> _list2 = new List<AuthToken>();

		public AuthToken_EqualityOperators()
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
			AuthToken authToken = new AuthToken();
			Assert.False(authToken == null);
			Assert.False(null == authToken);
			Assert.True(authToken != null);
			Assert.True(null != authToken);
			authToken = null;
			Assert.True(authToken == null);
			Assert.True(null == authToken);
			Assert.False(authToken != null);
			Assert.False(null != authToken);
		}

		[Fact]
		public void AgreesWithEquals()
		{
			for (int i = 0; i < _list1.Count; i++)
				for (int j = 0; j < _list1.Count; j++)
				{
					var at1 = _list1[i];
					var at2 = _list2[j];
					Assert.Equal(at1.Equals(at2), at1 == at2);
					Assert.Equal(!at1.Equals(at2), at1 != at2);
				}
		}
	}
}