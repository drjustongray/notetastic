using System;
using System.Linq;
using NotetasticApi.Common;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRepositoryTests
{
	public class UserRepository_UpdatePassword : UserRepository_Base
	{
		public UserRepository_UpdatePassword(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfIdNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.UpdatePasswordHash(null, "asdf@gasd")
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ShouldThrowIfUserNameNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.UpdatePasswordHash("asdfds", null)
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
			user.PasswordHash = "12zxvxcvdg3";
			_expectedUsers.Add(user);
			await _repo.UpdatePasswordHash(user.Id, user.PasswordHash);
			AssertCollectionEquals();

		}

		[Fact]
		public async void ReturnsUser()
		{
			foreach (var aUser in _expectedUsers)
			{
				aUser.PasswordHash = "12zxvxcvdg3";
				var user = await _repo.UpdatePasswordHash(aUser.Id, aUser.PasswordHash);
				Assert.Equal(aUser, user);
			}
		}

		[Fact]
		public async void ThrowsIfUserDoesNotExist()
		{
			await Assert.ThrowsAsync<DocumentNotFoundException>(
				() => _repo.UpdatePasswordHash("asdf", "asdfld")
			);
			AssertCollectionEquals();
		}
	}
}