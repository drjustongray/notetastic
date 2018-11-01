using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class AuthController_Register : AuthController_Base
	{
		public AuthController_Register()
		{
			var userOut = "";
			validationService.Setup(x => x.IsUsernameValid(It.IsNotNull<string>(), out userOut)).Returns(true);
			userOut = "Username missing";
			validationService.Setup(x => x.IsUsernameValid(null, out userOut)).Returns(false);

			var passwordOut = "";
			validationService.Setup(x => x.IsPasswordValid(It.IsNotNull<string>(), out passwordOut)).Returns(true);
			passwordOut = "Password missing";
			validationService.Setup(x => x.IsPasswordValid(null, out passwordOut)).Returns(false);
		}

		[Fact]
		public async void PerformsNullCheck()
		{
			var result = await authController.Register(null);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("password", modelState.Keys);
			Assert.Contains("username", modelState.Keys);
			Assert.Equal(2, modelState.Count);

			var passwordErrors = modelState["password"] as IEnumerable<string>;
			Assert.Contains("Password missing", passwordErrors);
			var usernameErrors = modelState["username"] as IEnumerable<string>;
			Assert.Contains("Username missing", usernameErrors);
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username1", "password2")]
		[InlineData("username1", "password3")]
		public async void ValidatesUsername(string username, string password)
		{
			var reason = "some made up reason";
			validationService.Setup(x => x.IsUsernameValid(username, out reason)).Returns(false);
			var authRequest = new AuthenticationRequest
			{
				username = username,
				password = password
			};
			var result = await authController.Register(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("username", modelState.Keys);
			Assert.Single(modelState);
			var usernameErrors = modelState["username"] as IEnumerable<string>;
			Assert.Contains(reason, usernameErrors);
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username2", "password2")]
		[InlineData("username3", "password3")]
		public async void ValidatesPassword(string username, string password)
		{
			var reason = "green goblin";
			validationService.Setup(x => x.IsPasswordValid(password, out reason)).Returns(false);
			var authRequest = new AuthenticationRequest
			{
				username = username,
				password = password
			};
			var result = await authController.Register(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("password", modelState.Keys);
			Assert.Single(modelState);
			var passwordErrors = modelState["password"] as IEnumerable<string>;
			Assert.Contains(reason, passwordErrors);
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username2", "password2")]
		[InlineData("username3", "password3")]
		public async void ValidatesBoth(string username, string password)
		{
			var reason1 = "some new made up reason";
			validationService.Setup(x => x.IsUsernameValid(username, out reason1)).Returns(false);
			var reason2 = "red goblin";
			validationService.Setup(x => x.IsPasswordValid(password, out reason2)).Returns(false);
			var authRequest = new AuthenticationRequest
			{
				username = username,
				password = password
			};
			var result = await authController.Register(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("password", modelState.Keys);
			Assert.Contains("username", modelState.Keys);
			Assert.Equal(2, modelState.Count);

			var passwordErrors = modelState["password"] as IEnumerable<string>;
			Assert.Contains(reason2, passwordErrors);
			var usernameErrors = modelState["username"] as IEnumerable<string>;
			Assert.Contains(reason1, usernameErrors);
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username3", "password2")]
		[InlineData("username3", "password3")]
		public async void ReturnsConflictIfRegistrationFails(string username, string password)
		{
			var authRequest = new AuthenticationRequest { username = username, password = password };
			userService.Setup(x => x.CreateAccount(username, password))
				.ReturnsAsync((User)null);

			var result = await authController.Register(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			Assert.IsType<ConflictResult>(actionResult.Result);
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username2", "password2")]
		[InlineData("username3", "password3")]
		public async void ReturnsAuthenticationResponseIfSuccessful(string username, string password)
		{
			var authReq = new AuthenticationRequest { username = username, password = password };
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";
			userService.Setup(x => x.CreateAccount(username, password))
				.ReturnsAsync(user);
			userService.Setup(x => x.CreateAuthTokens(user, It.IsAny<bool>()))
				.ReturnsAsync(new TokenPair { AccessToken = accessToken, RefreshToken = refToken });

			foreach (var rememberMe in new bool?[] { null, true, false })
			{
				SetupContext(SetupResponseCookies().Object);

				authReq.rememberMe = rememberMe;
				var result = await authController.Register(authReq);
				var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
				var authRes = Assert.IsType<AuthenticationResponse>(actionResult.Value);
				Assert.Equal(user.Id, authRes.uid);
				Assert.Equal(username, authRes.username);
				Assert.Equal(accessToken, authRes.token);
			}
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username3", "password2")]
		[InlineData("username3", "password3")]
		public async void AddsSessionCookieIfSuccessful(string username, string password)
		{
			var authReq = new AuthenticationRequest { username = username, password = password };
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";
			userService.Setup(x => x.CreateAccount(username, password))
				.ReturnsAsync(user);
			userService.Setup(x => x.CreateAuthTokens(user, false))
				.ReturnsAsync(new TokenPair { AccessToken = accessToken, RefreshToken = refToken });

			foreach (var rememberMe in new bool?[] { null, false })
			{
				var cookies = SetupResponseCookies();
				SetupContext(cookies.Object);

				authReq.rememberMe = rememberMe;
				await authController.Register(authReq);
				cookies.Verify(x => x.Append(AuthController.REFRESH_TOKEN, refToken, It.Is<CookieOptions>(
					_ => _.Secure == true &&
						_.HttpOnly == true &&
						_.MaxAge == null &&
						_.Expires == null
				)), Times.Once);
				cookies.VerifyNoOtherCalls();
			}
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username3", "password2")]
		[InlineData("username3", "password3")]
		public async void AddsPermCookieIfRememberMeTrue(string username, string password)
		{
			var authReq = new AuthenticationRequest { username = username, password = password };
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";
			userService.Setup(x => x.CreateAccount(username, password))
				.ReturnsAsync(user);
			userService.Setup(x => x.CreateAuthTokens(user, true))
				.ReturnsAsync(new TokenPair { AccessToken = accessToken, RefreshToken = refToken });

			var cookies = SetupResponseCookies();
			SetupContext(cookies.Object);

			authReq.rememberMe = true;
			await authController.Register(authReq);
			cookies.Verify(x => x.Append(AuthController.REFRESH_TOKEN, refToken, It.Is<CookieOptions>(
					_ => _.Secure == true &&
						_.HttpOnly == true &&
						_.MaxAge == TimeSpan.FromDays(30)
				)), Times.Once);
			cookies.VerifyNoOtherCalls();
		}
	}
}