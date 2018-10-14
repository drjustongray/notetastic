using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class UserController_GetUserAuth : UserController_Base
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public async void ReturnsBadRequestIfCookieMissing(string value)
		{
			SetupRequestCookies(value);
			var result = await userController.GetUserAuth();
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			Assert.IsType<BadRequestResult>(actionResult.Result);
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void ReturnsUnauthorizedIfTokenBad(string token)
		{
			SetupRequestCookies(token);
			userService.Setup(x => x.CreateAuthTokens(token)).ReturnsAsync((TokenPair)null);
			var result = await userController.GetUserAuth();
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			Assert.IsType<UnauthorizedResult>(actionResult.Result);
		}

		[Theory]
		[InlineData("token1", "username1")]
		[InlineData("token2", "username2")]
		[InlineData("token3", "username3")]
		public async void ReturnsAuthenticationResponseIfTokenGood(string token, string username)
		{
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";

			var cookies = SetupRequestCookies(token, true);
			cookies.Setup(x => x.Append(UserController.REFRESH_TOKEN, refToken, It.IsAny<CookieOptions>()));
			userService.Setup(x => x.CreateAuthTokens(token))
				.ReturnsAsync(new TokenPair { AccessToken = accessToken, RefreshToken = refToken, User = user });
			var result = await userController.GetUserAuth();
			var actionResult = Assert.IsType<ActionResult<AuthenticationResponse>>(result);
			var authRes = Assert.IsType<AuthenticationResponse>(actionResult.Value);
			Assert.Equal(user.Id, authRes.uid);
			Assert.Equal(username, authRes.username);
			Assert.Equal(accessToken, authRes.token);
		}

		[Theory]
		[InlineData("token1", "username1")]
		[InlineData("token2", "username2")]
		[InlineData("token3", "username3")]
		public async void AddsSessionCookieIfTokenGood(string token, string username)
		{
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";

			var cookies = SetupRequestCookies(token, true);
			cookies.Setup(x => x.Append(UserController.REFRESH_TOKEN, refToken, It.IsAny<CookieOptions>()));
			userService.Setup(x => x.CreateAuthTokens(token))
				.ReturnsAsync(new TokenPair
				{
					AccessToken = accessToken,
					RefreshToken = refToken,
					User = user,
					Persistent = false
				});
			await userController.GetUserAuth();
			cookies.Verify(x => x.Append(UserController.REFRESH_TOKEN, refToken, It.Is<CookieOptions>(
					_ => _.Secure == true &&
						_.HttpOnly == true &&
						_.MaxAge == null &&
						_.Expires == null
				)), Times.Once);
			cookies.VerifyNoOtherCalls();
		}

		[Theory]
		[InlineData("token1", "username1")]
		[InlineData("token2", "username2")]
		[InlineData("token3", "username3")]
		public async void AddsPermCookieIfTokenGoodAndPersistent(string token, string username)
		{
			var user = new User { Id = "someid", UserName = username };
			var refToken = "asdflasdfas";
			var accessToken = "78a0sd8v098as08dv";

			var cookies = SetupRequestCookies(token, true);
			cookies.Setup(x => x.Append(UserController.REFRESH_TOKEN, refToken, It.IsAny<CookieOptions>()));
			userService.Setup(x => x.CreateAuthTokens(token))
				.ReturnsAsync(new TokenPair
				{
					AccessToken = accessToken,
					RefreshToken = refToken,
					User = user,
					Persistent = true
				});
			await userController.GetUserAuth();
			cookies.Verify(x => x.Append(UserController.REFRESH_TOKEN, refToken, It.Is<CookieOptions>(
					_ => _.Secure == true &&
						_.HttpOnly == true &&
						_.MaxAge == TimeSpan.FromDays(30)
				)), Times.Once);
			cookies.VerifyNoOtherCalls();
		}
	}
}