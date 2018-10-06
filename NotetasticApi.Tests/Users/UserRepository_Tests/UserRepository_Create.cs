using System;
using NotetasticApi.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRepository_Tests
{
	public class UserRepository_Create : UserRepository_Base
	{
		public UserRepository_Create(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ShouldThrowIfArgNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repo.Create(null)
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ShouldThrowIfUserInvalid()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Create(new User())
			);
			AssertCollectionEquals();
		}


		[Theory]
		[InlineData("null")]
		[InlineData("")]
		public async void ShouldThrowIfIdNonNull(string value)
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => _repo.Create(new User { UserName = "f@gd", PasswordHash = "value", Id = value })
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("jane")]
		[InlineData("bob")]
		public async void ShouldThrowIfUserNameInUse(string username)
		{
			await Assert.ThrowsAsync<DocumentConflictException>(
				() => _repo.Create(new User { UserName = username, PasswordHash = "asdfasdf" })
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("asdf", "asdfd")]
		[InlineData("ad089708", "as0d8f9c")]
		public async void ShouldAddDocToDB(string userName, string passwordHash)
		{
			var user = new User { UserName = userName, PasswordHash = passwordHash };
			await _repo.Create(user);

			_expectedUsers.Add(user);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("asdf", "asdfd")]
		[InlineData("ad089708", "as0d8f9c")]
		public async void ShouldReturnUserDoc(string userName, string passwordHash)
		{
			var user = new User { UserName = userName, PasswordHash = passwordHash };
			var aUser = await _repo.Create(user);

			Assert.Equal(user, aUser);
		}
	}
}