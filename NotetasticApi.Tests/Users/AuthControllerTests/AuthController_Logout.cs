using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class AuthController_Logout : AuthController_Base
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public async void ReturnsBadRequestIfCookieMissing(string token)
		{
			SetupContext(reqCookies: SetupRequestCookies(token));
			var result = await authController.Logout();
			Assert.IsType<BadRequestResult>(result);
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void ResultIsNoContent(string token)
		{
			SetupContext(SetupResponseCookies().Object, SetupRequestCookies(token));
			userService.Setup(x => x.RevokeRefreshToken(token)).Returns(Task.CompletedTask);
			var result = await authController.Logout();
			Assert.IsType<NoContentResult>(result);
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void RevokesRefreshToken(string token)
		{
			SetupContext(SetupResponseCookies().Object, SetupRequestCookies(token));
			userService.Setup(x => x.RevokeRefreshToken(token)).Returns(Task.CompletedTask);
			await authController.Logout();
			userService.Verify(x => x.RevokeRefreshToken(token));
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void RemovesRefreshTokenCookie(string token)
		{
			var cookies = SetupResponseCookies();
			SetupContext(cookies.Object, SetupRequestCookies(token));
			userService.Setup(x => x.RevokeRefreshToken(token)).Returns(Task.CompletedTask);
			await authController.Logout();
			cookies.Verify(x => x.Delete(AuthController.REFRESH_TOKEN));
		}
	}
}