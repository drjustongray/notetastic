using System;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_Authenticate : UserService_Base
	{
		public UserService_Authenticate()
		{
			validationService.Setup(x => x.AssertNonNull<string>(It.IsNotNull<string>(), It.IsNotNull<string>()));
		}

		[Fact]
		public async void ValidatesUsername()
		{
			var expected = new ArgumentNullException("username");
			validationService.Setup(x => x.AssertNonNull<string>(null, "username"))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentNullException>(
				() => userService.Authenticate(null, "asdf")
			);
			Assert.Same(expected, actual);
		}

		[Fact]
		public async void ValidatesPassword()
		{
			var expected = new ArgumentNullException("password");
			validationService.Setup(x => x.AssertNonNull<string>(null, "password"))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentNullException>(
				() => userService.Authenticate("ausdfsd", null)
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu")]
		[InlineData("7907", "password3ajsdlkfj")]
		public async void ReturnsNullIfPasswordDoesNotMatch(string username, string password)
		{
			var hash = password + "hash";
			var expected = new User
			{
				Id = "idthingy",
				UserName = username,
				PasswordHash = hash
			};

			userRepo.Setup(x => x.FindByUserName(username)).ReturnsAsync(expected);
			passwordService.Setup(x => x.Verify(password, hash)).Returns(false);

			var actual = await userService.Authenticate(username, password);
			Assert.Null(actual);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu")]
		[InlineData("7907", "password3ajsdlkfj")]
		public async void ReturnsNullIfUserNotFound(string username, string password)
		{
			userRepo.Setup(x => x.FindByUserName(username)).ReturnsAsync((User)null);
			var actual = await userService.Authenticate(username, password);
			Assert.Null(actual);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu")]
		[InlineData("7907", "password3ajsdlkfj")]
		public async void ReturnsUserIfPasswordMatched(string username, string password)
		{
			var hash = password + "hash";
			var expected = new User
			{
				Id = "idthingy",
				UserName = username,
				PasswordHash = hash
			};

			userRepo.Setup(x => x.FindByUserName(username)).ReturnsAsync(expected);
			passwordService.Setup(x => x.Verify(password, hash)).Returns(true);

			var actual = await userService.Authenticate(username, password);
			Assert.Equal(expected, actual);
		}
	}
}