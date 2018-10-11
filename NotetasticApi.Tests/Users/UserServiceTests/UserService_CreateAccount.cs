using System;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_CreateAccount : UserService_Base
	{
		public UserService_CreateAccount()
		{
			validationService.Setup(
				x => x.ValidateUsername(It.IsAny<string>())
			);
			validationService.Setup(
				x => x.ValidatePassword(It.IsAny<string>())
			);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("asdfasdf")]
		[InlineData("afsd")]
		[InlineData("7907")]
		public async void ValidatesUsername(string username)
		{
			var expected = new Exception();
			validationService.Setup(
				x => x.ValidateUsername(It.IsAny<string>())
			).Throws(expected);

			var actual = await Assert.ThrowsAsync<Exception>(
				() => userService.CreateAccount(username, "")
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("asdfasdf")]
		[InlineData("afsd")]
		[InlineData("7907")]
		public async void ValidatesPassword(string password)
		{
			var expected = new Exception();
			validationService.Setup(
				x => x.ValidatePassword(It.IsAny<string>())
			).Throws(expected);

			var actual = await Assert.ThrowsAsync<Exception>(
				() => userService.CreateAccount("username", password)
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("asdfasdf", "password11kla;jdflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu")]
		[InlineData("7907", "password3ajsdlkfj")]
		public async void SavesUserDocToDB(string username, string password)
		{
			var hash = password + "hash";
			passwordService.Setup(x => x.Hash(password)).Returns(hash);
			var expectedUser = new User
			{
				UserName = username,
				PasswordHash = hash
			};

			userRepo.Setup(x => x.Create(expectedUser)).ReturnsAsync(new User());

			await userService.CreateAccount(username, password);
			userRepo.Verify(x => x.Create(expectedUser));
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("asdfasdf", "password11kla;jdflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu")]
		[InlineData("7907", "password3ajsdlkfj")]
		public async void ReturnsUserDoc(string username, string password)
		{
			var hash = password + "hash";
			passwordService.Setup(x => x.Hash(password)).Returns(hash);
			var user = new User
			{
				UserName = username,
				PasswordHash = hash
			};

			var expected = new User
			{
				UserName = username,
				PasswordHash = hash,
				Id = "someIdasdf"
			};

			userRepo.Setup(x => x.Create(user)).ReturnsAsync(expected);

			var actual = await userService.CreateAccount(username, password);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("asdfasdf", "password11kla;jdflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu")]
		[InlineData("7907", "password3ajsdlkfj")]
		public async void ThrowsIfUsernameConflict(string username, string password)
		{
			var hash = password + "hash";
			passwordService.Setup(x => x.Hash(password)).Returns(hash);
			var user = new User
			{
				UserName = username,
				PasswordHash = hash
			};

			var expected = new Exception();

			userRepo.Setup(x => x.Create(user)).ThrowsAsync(expected);

			var actual = await Assert.ThrowsAsync<Exception>(
				() => userService.CreateAccount(username, password)
			);
			Assert.Same(expected, actual);
		}
	}
}