using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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

		protected Mock<IResponseCookies> SetupResponseCookies(string uid = null)
		{
			var controllerContext = new ControllerContext();
			var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
			if (uid != null)
			{
				httpContext.SetupGet(x => x.User).Returns(
					new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(NotetasticApi.Users.ClaimTypes.UID, uid) }))
				);
			}
			var response = new Mock<HttpResponse>(MockBehavior.Strict);
			var cookies = new Mock<IResponseCookies>(MockBehavior.Strict);
			response.SetupGet(x => x.Cookies).Returns(cookies.Object);
			httpContext.SetupGet(x => x.Response).Returns(response.Object);
			controllerContext.HttpContext = httpContext.Object;
			userController.ControllerContext = controllerContext;
			return cookies;
		}

		protected Mock<IResponseCookies> SetupRequestCookies(string refreshToken = null, bool includeResponse = false)
		{
			var controllerContext = new ControllerContext();
			var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
			var request = new Mock<HttpRequest>(MockBehavior.Strict);
			//var cookies = new Mock<IResponseCookies>(MockBehavior.Strict);
			var cookies = refreshToken == null ?
				new RequestCookieCollection() :
				new RequestCookieCollection(new Dictionary<string, string> { { UserController.REFRESH_TOKEN, refreshToken } });

			request.SetupGet(x => x.Cookies).Returns(cookies);
			httpContext.SetupGet(x => x.Request).Returns(request.Object);

			Mock<IResponseCookies> resCookies = null;
			if (includeResponse)
			{
				var response = new Mock<HttpResponse>(MockBehavior.Strict);
				resCookies = new Mock<IResponseCookies>(MockBehavior.Strict);
				response.SetupGet(x => x.Cookies).Returns(resCookies.Object);
				httpContext.SetupGet(x => x.Response).Returns(response.Object);
			}
			controllerContext.HttpContext = httpContext.Object;
			userController.ControllerContext = controllerContext;

			return resCookies;
		}

	}
}