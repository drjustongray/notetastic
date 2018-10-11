using System;
using Moq;
using NotetasticApi.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_ChangeUsername : UserService_Base
	{
		public UserService_ChangeUsername()
		{
			validationService.Setup(x => x.AssertNonNull<string>(It.IsNotNull<string>(), It.IsNotNull<string>()));
			validationService.Setup(x => x.ValidateUsername(It.IsNotNull<string>()));
		}

		[Fact]
		public async void ValidatesUid()
		{
			var expected = new ArgumentNullException("uid");
			validationService.Setup(x => x.AssertNonNull<string>(null, "uid"))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentNullException>(
				() => userService.ChangeUsername(null, "asdf", "passwrod")
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
				() => userService.ChangeUsername("ausdfsd", "usernameasdf", null)
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData("fasdf")]
		[InlineData(null)]
		[InlineData("fasdkj ;lfaksdj")]
		public async void ValidatesUsername(string username)
		{
			var expected = new ArgumentException("username");
			validationService.Setup(x => x.ValidateUsername(username))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentException>(
				() => userService.ChangeUsername("ausdfsd", username, "paasdfd")
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData("asdfasdf", "user;jdflkjsd", "password11kflkjsd")]
		[InlineData("afsd", "user2aoiudvfoiu", "passworlkjsd")]
		[InlineData("7907", "user3ajsdlkfj", "passwojdflkjsd")]
		public async void ThrowsIfUserNotFound(string uid, string username, string password)
		{
			userRepo.Setup(x => x.FindById(uid)).ReturnsAsync((User)null);

			await Assert.ThrowsAsync<DocumentNotFoundException>(
				() => userService.ChangeUsername(uid, username, password)
			);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd", "password11kflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu", "passworlkjsd")]
		[InlineData("7907", "password3ajsdlkfj", "passwojdflkjsd")]
		public async void ReturnsNullIfPasswordWrong(string uid, string username, string password)
		{
			var hash = password + "hash";
			var user = new User
			{
				Id = uid,
				UserName = "dontmatter",
				PasswordHash = hash
			};
			userRepo.Setup(x => x.FindById(uid)).ReturnsAsync(user);
			passwordService.Setup(x => x.Verify(password, hash)).Returns(false);

			var result = await userService.ChangeUsername(uid, username, password);
			Assert.Null(result);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd", "password11kflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu", "passworlkjsd")]
		[InlineData("7907", "password3ajsdlkfj", "passwojdflkjsd")]
		public async void ReturnsUserAndSavesDocIfPasswordRight(string uid, string username, string password)
		{
			var hash = password + "hash";
			var user = new User
			{
				Id = uid,
				UserName = "dontmatter",
				PasswordHash = hash
			};

			var expected = new User
			{
				Id = uid,
				UserName = username,
				PasswordHash = hash
			};
			userRepo.Setup(x => x.FindById(uid)).ReturnsAsync(user);
			userRepo.Setup(x => x.UpdateUserName(uid, username)).ReturnsAsync(expected);

			passwordService.Setup(x => x.Verify(password, hash)).Returns(true);

			var actual = await userService.ChangeUsername(uid, username, password);
			Assert.Equal(expected, actual);
		}
	}
}