using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class UserController_Logout : UserController_Base
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public async void ReturnsBadRequestIfCookieMissing(string token)
		{
			SetupRequestCookies(token);
			var result = await userController.Logout();
			Assert.IsType<BadRequestResult>(result);
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void ResultIsNoContent(string token)
		{
			var cookies = SetupRequestCookies(token, true);
			cookies.Setup(x => x.Delete(UserController.REFRESH_TOKEN));
			userService.Setup(x => x.RevokeRefreshToken(token)).Returns(Task.CompletedTask);
			var result = await userController.Logout();
			Assert.IsType<NoContentResult>(result);
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void RevokesRefreshToken(string token)
		{
			var cookies = SetupRequestCookies(token, true);
			cookies.Setup(x => x.Delete(UserController.REFRESH_TOKEN));
			userService.Setup(x => x.RevokeRefreshToken(token)).Returns(Task.CompletedTask);
			await userController.Logout();
			userService.Verify(x => x.RevokeRefreshToken(token));
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void RemovesRefreshTokenCookie(string token)
		{
			var cookies = SetupRequestCookies(token, true);
			cookies.Setup(x => x.Delete(UserController.REFRESH_TOKEN));
			userService.Setup(x => x.RevokeRefreshToken(token)).Returns(Task.CompletedTask);
			await userController.Logout();
			cookies.Verify(x => x.Delete(UserController.REFRESH_TOKEN));
		}
	}
}