using System;
using Moq;
using NotetasticApi.Common;
using NotetasticApi.Users;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_Base
	{
		protected readonly Mock<IUserRepository> userRepo;
		protected readonly Mock<IRefreshTokenRepository> refreshTokenRepo;
		protected readonly Mock<IPasswordService> passwordService;
		protected readonly Mock<ITokenService> tokenService;
		protected readonly Mock<IValidationService> validationService;
		protected readonly Mock<ITimeService> timeService;
		protected readonly UserService userService;
		protected readonly DateTimeOffset Now = DateTimeOffset.Now;

		public UserService_Base()
		{
			userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
			refreshTokenRepo = new Mock<IRefreshTokenRepository>(MockBehavior.Strict);
			passwordService = new Mock<IPasswordService>(MockBehavior.Strict);
			tokenService = new Mock<ITokenService>(MockBehavior.Strict);
			validationService = new Mock<IValidationService>(MockBehavior.Strict);
			timeService = new Mock<ITimeService>();
			timeService.Setup(x => x.GetCurrentTime()).Returns(Now);

			userService = new UserService(
				userRepo.Object,
				refreshTokenRepo.Object,
				passwordService.Object,
				tokenService.Object,
				validationService.Object,
				timeService.Object
			);
		}

	}
}