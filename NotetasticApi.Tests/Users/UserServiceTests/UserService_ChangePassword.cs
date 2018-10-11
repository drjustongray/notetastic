using System;
using Moq;
using NotetasticApi.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_ChangePassword : UserService_Base
	{
		public UserService_ChangePassword()
		{
			validationService.Setup(x => x.AssertNonNull<string>(It.IsNotNull<string>(), It.IsNotNull<string>()));
			validationService.Setup(x => x.ValidatePassword(It.IsNotNull<string>()));
		}

		[Fact]
		public async void ValidatesUid()
		{
			var expected = new ArgumentNullException("uid");
			validationService.Setup(x => x.AssertNonNull<string>(null, "uid"))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentNullException>(
				() => userService.ChangePassword(null, "asdf", "passwrod")
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
				() => userService.ChangePassword("ausdfsd", null, "passwere")
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData("fasdf")]
		[InlineData(null)]
		[InlineData("fasdkj ;lfaksdj")]
		public async void ValidatesNewPassword(string password)
		{
			var expected = new ArgumentException("newPassword");
			validationService.Setup(x => x.ValidatePassword(password))
				.Throws(expected);
			var actual = await Assert.ThrowsAsync<ArgumentException>(
				() => userService.ChangePassword("ausdfsd", "paasdfd", password)
			);
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd", "password11kflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu", "passworlkjsd")]
		[InlineData("7907", "password3ajsdlkfj", "passwojdflkjsd")]
		public async void ThrowsIfUserNotFound(string uid, string password, string newPassword)
		{
			userRepo.Setup(x => x.FindById(uid)).ReturnsAsync((User)null);

			await Assert.ThrowsAsync<DocumentNotFoundException>(
				() => userService.ChangePassword(uid, password, newPassword)
			);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd", "password11kflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu", "passworlkjsd")]
		[InlineData("7907", "password3ajsdlkfj", "passwojdflkjsd")]
		public async void ReturnsNullIfPasswordWrong(string uid, string password, string newPassword)
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

			var result = await userService.ChangePassword(uid, password, newPassword);
			Assert.Null(result);
		}

		[Theory]
		[InlineData("asdfasdf", "password11kla;jdflkjsd", "password11kflkjsd")]
		[InlineData("afsd", "password2aoiudvfoiu", "passworlkjsd")]
		[InlineData("7907", "password3ajsdlkfj", "passwojdflkjsd")]
		public async void ReturnsUserAndSavesDocIfPasswordRight(string uid, string password, string newPassword)
		{
			var hash = password + "hash";
			var user = new User
			{
				Id = uid,
				UserName = "dontmatter",
				PasswordHash = hash
			};

			var newHash = newPassword + "hash";
			var expected = new User
			{
				Id = uid,
				UserName = "dontmatter",
				PasswordHash = newHash
			};
			userRepo.Setup(x => x.FindById(uid)).ReturnsAsync(user);
			userRepo.Setup(x => x.UpdatePasswordHash(uid, newHash)).ReturnsAsync(expected);

			passwordService.Setup(x => x.Verify(password, hash)).Returns(true);
			passwordService.Setup(x => x.Hash(newPassword)).Returns(newHash);

			var actual = await userService.ChangePassword(uid, password, newPassword);
			Assert.Equal(expected, actual);
		}
	}
}