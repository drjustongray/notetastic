using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace NotetasticApi.Tests.Users.UserServiceTests
{
	public class UserService_RevokeRefreshToken : UserService_Base
	{
		[Fact]
		public async void ValidatesTokenArg()
		{
			var e = new Exception();
			validationService.Setup(x => x.AssertNonNull(null, "token"))
				.Throws(e);

			var ex = await Assert.ThrowsAnyAsync<Exception>(
				() => userService.RevokeRefreshToken(null)
			);
			Assert.Same(e, ex);
		}

		[Theory]
		[InlineData("token1")]
		[InlineData("token2")]
		[InlineData("token3")]
		public async void DeletesToken(string token)
		{
			validationService.Setup(x => x.AssertNonNull(It.IsNotNull<string>(), "token"));
			refreshTokenRepo.Setup(x => x.Delete(token)).Returns(Task.CompletedTask);
			await userService.RevokeRefreshToken(token);
			refreshTokenRepo.Verify(x => x.Delete(token), Times.Once);
		}
	}
}