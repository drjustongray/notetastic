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
	public class AuthController_Base
	{
		protected readonly Mock<IUserService> userService;
		protected readonly Mock<IValidationService> validationService;
		protected readonly AuthController authController;

		public AuthController_Base()
		{
			userService = new Mock<IUserService>(MockBehavior.Strict);
			validationService = new Mock<IValidationService>(MockBehavior.Strict);
			authController = new AuthController(userService.Object, validationService.Object);
		}

		protected void SetupContext(IResponseCookies resCookies = null, IRequestCookieCollection reqCookies = null, string uid = null)
		{
			var controllerContext = new ControllerContext();
			var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
			if (resCookies != null)
			{
				var response = new Mock<HttpResponse>(MockBehavior.Strict);
				response.SetupGet(x => x.Cookies).Returns(resCookies);
				httpContext.SetupGet(x => x.Response).Returns(response.Object);
			}
			if (reqCookies != null)
			{
				var request = new Mock<HttpRequest>(MockBehavior.Strict);
				request.SetupGet(x => x.Cookies).Returns(reqCookies);
				httpContext.SetupGet(x => x.Request).Returns(request.Object);
			}
			if (uid != null)
			{
				httpContext.SetupGet(x => x.User).Returns(
					new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(NotetasticApi.Users.ClaimTypes.UID, uid) }))
				);
			}
			controllerContext.HttpContext = httpContext.Object;
			authController.ControllerContext = controllerContext;
		}

		protected Mock<IResponseCookies> SetupResponseCookies()
		{
			var cookies = new Mock<IResponseCookies>(MockBehavior.Strict);
			cookies.Setup(x => x.Append(AuthController.REFRESH_TOKEN, It.IsAny<string>(), It.IsAny<CookieOptions>()));
			cookies.Setup(x => x.Delete(AuthController.REFRESH_TOKEN));
			return cookies;
		}

		protected IRequestCookieCollection SetupRequestCookies(string refreshToken = null)
		{
			return refreshToken == null ?
				new RequestCookieCollection() :
				new RequestCookieCollection(new Dictionary<string, string> { { AuthController.REFRESH_TOKEN, refreshToken } });
		}

	}
}