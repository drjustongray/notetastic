using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class AuthController_LogoutAll : AuthController_Base
	{
		private Mock<IResponseCookies> cookies;

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		[InlineData("uid3")]
		public async void RevokesAllTokens(string uid)
		{
			Setup(uid);
			await authController.LogoutAll();
			userService.Verify(x => x.RevokeAllRefreshTokens(uid), Times.Once);
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		[InlineData("uid3")]
		public async void DeletesCookie(string uid)
		{
			Setup(uid);
			await authController.LogoutAll();
			cookies.Verify(x => x.Delete(AuthController.REFRESH_TOKEN));
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		[InlineData("uid3")]
		public async void ReturnsNoContent(string uid)
		{
			Setup(uid);
			var result = await authController.LogoutAll();
			Assert.IsType<NoContentResult>(result);
		}

		private void Setup(string uid)
		{
			userService.Setup(x => x.RevokeAllRefreshTokens(uid))
				.Returns(Task.CompletedTask);
			cookies = SetupResponseCookies();
			SetupContext(cookies.Object, uid: uid);
		}
	}
}