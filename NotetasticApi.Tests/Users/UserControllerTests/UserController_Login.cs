using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class UserController_Login : UserController_Base
	{
		[Fact]
		public async void PerformsNullCheck()
		{
			var result = await userController.Login(null);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("password", modelState.Keys);
			Assert.Contains("username", modelState.Keys);
			Assert.Equal(2, modelState.Count);
		}

		[Theory]
		[InlineData("password1")]
		[InlineData("password2")]
		[InlineData("password3")]
		public async void ChecksUsername(string password)
		{
			var authRequest = new AuthenticationRequest
			{
				password = password
			};
			var result = await userController.Login(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("username", modelState.Keys);
			Assert.Single(modelState);
		}

		[Theory]
		[InlineData("username1")]
		[InlineData("username2")]
		[InlineData("username3")]
		public async void ChecksPassword(string username)
		{
			var authRequest = new AuthenticationRequest
			{
				username = username
			};
			var result = await userController.Login(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("password", modelState.Keys);
			Assert.Single(modelState);
		}

		[Fact]
		public async void ChecksBoth()
		{
			var authRequest = new AuthenticationRequest();
			var result = await userController.Login(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			var modelState = Assert.IsType<SerializableError>(badRequest.Value);
			Assert.Contains("password", modelState.Keys);
			Assert.Contains("username", modelState.Keys);
			Assert.Equal(2, modelState.Count);
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username3", "password2")]
		[InlineData("username3", "password3")]
		public async void Returns401IfAuthenticationFails(string username, string password)
		{
			var authRequest = new AuthenticationRequest { username = username, password = password };
			userService.Setup(x => x.Authenticate(username, password))
				.ReturnsAsync((User)null);

			var result = await userController.Login(authRequest);
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			Assert.IsType<UnauthorizedResult>(actionResult.Result);
		}

		[Theory]
		[InlineData("username1", "password1")]
		[InlineData("username3", "password2")]
		[InlineData("username3", "password3")]
		public async void ReturnsAuthenticationResponseIfAuthenticationSucceeds(string username, string password)
		{
			var authReq = new AuthenticationRequest { username = username, password = password };
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";
			userService.Setup(x => x.Authenticate(username, password))
				.ReturnsAsync(user);
			userService.Setup(x => x.CreateAuthTokens(user, It.IsAny<bool>()))
				.ReturnsAsync(new TokenPair { AccessToken = accessToken, RefreshToken = refToken });

			foreach (var rememberMe in new bool?[] { null, true, false })
			{
				SetupContext(SetupResponseCookies().Object);

				authReq.rememberMe = rememberMe;
				var result = await userController.Login(authReq);
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
		public async void AddsSessionCookieIfAuthSucceeds(string username, string password)
		{
			var authReq = new AuthenticationRequest { username = username, password = password };
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";
			userService.Setup(x => x.Authenticate(username, password))
				.ReturnsAsync(user);
			userService.Setup(x => x.CreateAuthTokens(user, false))
				.ReturnsAsync(new TokenPair { AccessToken = accessToken, RefreshToken = refToken });

			foreach (var rememberMe in new bool?[] { null, false })
			{
				var cookies = SetupResponseCookies();
				SetupContext(cookies.Object);

				authReq.rememberMe = rememberMe;
				await userController.Login(authReq);
				cookies.Verify(x => x.Append(UserController.REFRESH_TOKEN, refToken, It.Is<CookieOptions>(
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
			userService.Setup(x => x.Authenticate(username, password))
				.ReturnsAsync(user);
			userService.Setup(x => x.CreateAuthTokens(user, true))
				.ReturnsAsync(new TokenPair { AccessToken = accessToken, RefreshToken = refToken });

			var cookies = SetupResponseCookies();
			SetupContext(cookies.Object);

			authReq.rememberMe = true;
			await userController.Login(authReq);
			cookies.Verify(x => x.Append(UserController.REFRESH_TOKEN, refToken, It.Is<CookieOptions>(
					_ => _.Secure == true &&
						_.HttpOnly == true &&
						_.MaxAge == TimeSpan.FromDays(30)
				)), Times.Once);
			cookies.VerifyNoOtherCalls();
		}
	}
}