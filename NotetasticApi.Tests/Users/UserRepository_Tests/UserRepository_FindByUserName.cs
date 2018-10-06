using System;
using System.Linq;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRepository_Tests
{
	public class UserRepository_FindByUserName : UserRepository_Base
	{
		public UserRepository_FindByUserName(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfArgumentNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.FindByUserName(null)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("jane")]
		[InlineData("bob")]
		public async void ReturnsUserIfFound(string userName)
		{
			var user = await _repo.FindByUserName(userName);
			Assert.Equal(user, _expectedUsers.First(_ => _.UserName == userName));
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsNullIfUserNotFound()
		{
			var user = await _repo.FindByUserName("jan6e");
			Assert.Null(user);
			AssertCollectionEquals();
		}
	}
}