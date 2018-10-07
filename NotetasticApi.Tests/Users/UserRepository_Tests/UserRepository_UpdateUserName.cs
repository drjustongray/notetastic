using System;
using System.Linq;
using NotetasticApi.Common;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRepository_Tests
{
	public class UserRepository_UpdateUserName : UserRepository_Base
	{
		public UserRepository_UpdateUserName(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfIdNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.UpdateUserName(null, "asdf@gasd")
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ShouldThrowIfUserNameNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.UpdateUserName("asdfds", null)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("jane")]
		[InlineData("bob")]
		public async void ModifiesCollectionCorrectly(string username)
		{
			var user = _expectedUsers.First(_ => _.UserName == username);
			_expectedUsers.Remove(user);
			user.UserName = username + "123";
			_expectedUsers.Add(user);
			await _repo.UpdateUserName(user.Id, user.UserName);
			AssertCollectionEquals();

		}

		[Fact]
		public async void ReturnsUser()
		{
			foreach (var aUser in _expectedUsers)
			{
				aUser.UserName = aUser.UserName + "n";
				var user = await _repo.UpdateUserName(aUser.Id, aUser.UserName);
				Assert.Equal(aUser, user);
			}
		}

		[Fact]
		public async void ThrowsIfUserDoesNotExist()
		{
			await Assert.ThrowsAsync<DocumentNotFoundException>(
				() => _repo.UpdateUserName("asdf", "asdfld")
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ThrowsIfUserNameInUse()
		{
			var user1 = _existing;
			var user2 = _otherExisting;
			await Assert.ThrowsAsync<DocumentConflictException>(
				() => _repo.UpdateUserName(user1.Id, user2.UserName)
			);
			await Assert.ThrowsAsync<DocumentConflictException>(
				() => _repo.UpdateUserName(user2.Id, user1.UserName)
			);
			AssertCollectionEquals();
		}
	}
}