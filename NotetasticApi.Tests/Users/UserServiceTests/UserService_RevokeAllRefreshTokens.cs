using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_RevokeAllRefreshTokens : UserService_Base
	{
		[Fact]
		public async void ValidatesUIDArg()
		{
			var e = new Exception();
			validationService.Setup(x => x.AssertNonNull(null, "uid"))
				.Throws(e);

			var ex = await Assert.ThrowsAnyAsync<Exception>(
				() => userService.RevokeAllRefreshTokens(null)
			);
			Assert.Same(e, ex);
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		[InlineData("uid3")]
		public async void DeletesAllTokens(string uid)
		{
			validationService.Setup(x => x.AssertNonNull(It.IsNotNull<string>(), "uid"));
			refreshTokenRepo.Setup(x => x.DeleteAll(uid)).Returns(Task.CompletedTask);
			await userService.RevokeAllRefreshTokens(uid);
			refreshTokenRepo.Verify(x => x.DeleteAll(uid), Times.Once);
		}
	}
}