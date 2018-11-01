using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class AuthController_UpdateUser : AuthController_Base
	{
		[Fact]
		public async void PerformsNullCheck()
		{
			var result = await authController.UpdateUser(null);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("message", modelState.Keys);
			Assert.Single(modelState);
		}

		[Theory]
		[InlineData(null, "something")]
		[InlineData("something", null)]
		public async void ChecksForPassword(string newUsername, string newPassword)
		{
			var authReq = new AuthenticationRequest
			{
				newPassword = newPassword,
				newUsername = newUsername
			};

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("message", modelState.Keys);
			Assert.Single(modelState);
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("something", "somethingelse")]
		public async void ChecksForPatchValues(string newUsername, string newPassword)
		{
			var authReq = new AuthenticationRequest
			{
				password = "password",
				newPassword = newPassword,
				newUsername = newUsername
			};

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("message", modelState.Keys);
			Assert.Single(modelState);
		}

		[Theory]
		[InlineData("user;jdflkjsd", "password11kflkjsd")]
		[InlineData("user2aoiudvfoiu", "passworlkjsd")]
		[InlineData("user3ajsdlkfj", "passwojdflkjsd")]
		public async void ValidatesUsername(string newUsername, string password)
		{
			var authReq = new AuthenticationRequest
			{
				password = password,
				newUsername = newUsername
			};
			var validationResponse = "thet's just wrong";
			validationService.Setup(x => x.IsUsernameValid(newUsername, out validationResponse))
				.Returns(false);

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("username", modelState.Keys);
			Assert.Single(modelState);
			Assert.Contains(validationResponse, modelState["username"] as IEnumerable<string>);
		}

		[Theory]
		[InlineData("uid1", "user;jdflkjsd", "password11kflkjsd")]
		[InlineData("uid2", "user2aoiudvfoiu", "passworlkjsd")]
		[InlineData("uid3", "user3ajsdlkfj", "passwojdflkjsd")]
		public async void UsernameUpdateReturnsUnauthorizedOnFail(string uid, string newUsername, string password)
		{
			var authReq = new AuthenticationRequest
			{
				password = password,
				newUsername = newUsername
			};
			var validationResponse = "";
			validationService.Setup(x => x.IsUsernameValid(newUsername, out validationResponse))
				.Returns(true);
			userService.Setup(x => x.ChangeUsername(uid, newUsername, password))
				.ReturnsAsync((User)null);
			SetupContext(uid: uid);

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			Assert.IsType<UnauthorizedResult>(actionResult.Result);
		}

		[Theory]
		[InlineData("uid1", "user;jdflkjsd", "password11kflkjsd")]
		[InlineData("uid2", "user2aoiudvfoiu", "passworlkjsd")]
		[InlineData("uid3", "user3ajsdlkfj", "passwojdflkjsd")]
		public async void UsernameUpdateReturnsAuthResOnSuccess(string uid, string newUsername, string password)
		{
			var authReq = new AuthenticationRequest
			{
				password = password,
				newUsername = newUsername
			};
			var validationResponse = "";
			validationService.Setup(x => x.IsUsernameValid(newUsername, out validationResponse))
				.Returns(true);
			userService.Setup(x => x.ChangeUsername(uid, newUsername, password))
				.ReturnsAsync(new User { Id = uid, UserName = newUsername });
			SetupContext(uid: uid);

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var authRes = Assert.IsType<AuthenticationResponse>(actionResult.Value);
			Assert.Equal(uid, authRes.uid);
			Assert.Equal(newUsername, authRes.username);
			Assert.Null(authRes.token);
		}

		[Theory]
		[InlineData(";jdflkjsd", "password11kflkjsd")]
		[InlineData("2aoiudvfoiu", "passworlkjsd")]
		[InlineData("3ajsdlkfj", "passwojdflkjsd")]
		public async void ValidatesPassword(string newPassword, string password)
		{
			var authReq = new AuthenticationRequest
			{
				password = password,
				newPassword = newPassword
			};
			var validationResponse = "thet's just wrong";
			validationService.Setup(x => x.IsPasswordValid(newPassword, out validationResponse))
				.Returns(false);

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("password", modelState.Keys);
			Assert.Single(modelState);
			Assert.Contains(validationResponse, modelState["password"] as IEnumerable<string>);
		}

		[Theory]
		[InlineData("uid1", ";jdflkjsd", "password11kflkjsd")]
		[InlineData("uid2", "2aoiudvfoiu", "passworlkjsd")]
		[InlineData("uid3", "3ajsdlkfj", "passwojdflkjsd")]
		public async void PasswordUpdateReturnsUnauthorizedOnFail(string uid, string newPassword, string password)
		{
			var authReq = new AuthenticationRequest
			{
				password = password,
				newPassword = newPassword
			};
			var validationResponse = "";
			validationService.Setup(x => x.IsPasswordValid(newPassword, out validationResponse))
				.Returns(true);
			userService.Setup(x => x.ChangePassword(uid, password, newPassword))
				.ReturnsAsync((User)null);
			SetupContext(uid: uid);

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			Assert.IsType<UnauthorizedResult>(actionResult.Result);
		}

		[Theory]
		[InlineData("uid1", ";jdflkjsd", "password11kflkjsd")]
		[InlineData("uid2", "2aoiudvfoiu", "passworlkjsd")]
		[InlineData("uid3", "3ajsdlkfj", "passwojdflkjsd")]
		public async void PasswordUpdateReturnsAuthResOnSuccess(string uid, string newPassword, string password)
		{
			var authReq = new AuthenticationRequest
			{
				password = password,
				newPassword = newPassword
			};
			var validationResponse = "";
			validationService.Setup(x => x.IsPasswordValid(newPassword, out validationResponse))
				.Returns(true);
			userService.Setup(x => x.ChangePassword(uid, password, newPassword))
				.ReturnsAsync(new User { Id = uid, UserName = uid + "something" });
			SetupContext(uid: uid);

			var result = await authController.UpdateUser(authReq);

			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var authRes = Assert.IsType<AuthenticationResponse>(actionResult.Value);
			Assert.Equal(uid, authRes.uid);
			Assert.Equal(uid + "something", authRes.username);
			Assert.Null(authRes.token);
		}
	}
}