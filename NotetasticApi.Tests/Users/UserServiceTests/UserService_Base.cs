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
		protected readonly UserService userService;

		public UserService_Base()
		{
			userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
			refreshTokenRepo = new Mock<IRefreshTokenRepository>(MockBehavior.Strict);
			passwordService = new Mock<IPasswordService>(MockBehavior.Strict);
			tokenService = new Mock<ITokenService>(MockBehavior.Strict);
			validationService = new Mock<IValidationService>(MockBehavior.Strict);
			userService = new UserService(
				userRepo.Object,
				refreshTokenRepo.Object,
				passwordService.Object,
				tokenService.Object,
				validationService.Object
			);
		}

		protected RefreshToken Matches(RefreshToken a)
		{
			return It.Is<RefreshToken>(_ => AlmostEqual(a, _));
		}

		protected bool AlmostEqual(RefreshToken a, RefreshToken b)
		{
			var timeDiff = a.ExpiresAt - b.ExpiresAt;

			if (Math.Abs(timeDiff?.Ticks ?? 10000000000) < 1000000)
			{
				return a.Equals(new RefreshToken { Id = b.Id, UID = b.UID, Token = b.Token, ExpiresAt = a.ExpiresAt });
			}
			return false;
		}
	}
}