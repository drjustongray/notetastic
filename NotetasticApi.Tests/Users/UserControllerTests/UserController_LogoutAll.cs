using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class UserController_LogoutAll : UserController_Base
	{
		private Mock<IResponseCookies> cookies;

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		[InlineData("uid3")]
		public async void RevokesAllTokens(string uid)
		{
			Setup(uid);
			await userController.LogoutAll();
			userService.Verify(x => x.RevokeAllRefreshTokens(uid), Times.Once);
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		[InlineData("uid3")]
		public async void DeletesCookie(string uid)
		{
			Setup(uid);
			await userController.LogoutAll();
			cookies.Verify(x => x.Delete(UserController.REFRESH_TOKEN));
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		[InlineData("uid3")]
		public async void ReturnsNoContent(string uid)
		{
			Setup(uid);
			var result = await userController.LogoutAll();
			Assert.IsType<NoContentResult>(result);
		}

		private void Setup(string uid)
		{
			userService.Setup(x => x.RevokeAllRefreshTokens(uid))
				.Returns(Task.CompletedTask);
			cookies = SetupResponseCookies(uid);
			cookies.Setup(x => x.Delete(UserController.REFRESH_TOKEN));
			
		}
	}
}