using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Common;
using NotetasticApi.Users;

namespace NotetasticApi.Tests.Users.UserControllerTests
{
	public class UserController_Base
	{
		protected readonly Mock<IUserService> userService;
		protected readonly Mock<IValidationService> validationService;
		protected readonly UserController userController;

		public UserController_Base()
		{
			userService = new Mock<IUserService>(MockBehavior.Strict);
			validationService = new Mock<IValidationService>(MockBehavior.Strict);
			userController = new UserController(userService.Object, validationService.Object);
		}

		protected Mock<IResponseCookies> SetupResponseCookies()
		{
			var controllerContext = new ControllerContext();
			var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
			var response = new Mock<HttpResponse>(MockBehavior.Strict);
			var cookies = new Mock<IResponseCookies>(MockBehavior.Strict);
			response.SetupGet(x => x.Cookies).Returns(cookies.Object);
			httpContext.SetupGet(x => x.Response).Returns(response.Object);
			controllerContext.HttpContext = httpContext.Object;
			userController.ControllerContext = controllerContext;
			return cookies;
		}
	}
}