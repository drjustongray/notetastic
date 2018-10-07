using System;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRepositoryTests
{
	public class UserRepository_FindById : UserRepository_Base
	{
		public UserRepository_FindById(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfArgumentNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.FindById(null)
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsUserIfFound()
		{
			foreach (var aUser in _expectedUsers)
			{
				var user = await _repo.FindById(aUser.Id);
				Assert.Equal(user, aUser);
			}
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsNullIfUserNotFound()
		{
			var user = await _repo.FindById("asdfasd");
			Assert.Null(user);
			AssertCollectionEquals();
		}
	}
}