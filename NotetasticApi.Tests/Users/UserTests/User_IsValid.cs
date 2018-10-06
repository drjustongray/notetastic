using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserTests
{
	public class User_IsValid
	{
		[Theory]
		[InlineData(null)]
		[InlineData("    ")]
		[InlineData("\n\t")]
		public void IsFalseIfUserNameInvalid(string username)
		{
			var user = new User
			{
				UserName = username,
				PasswordHash = "somehash"
			};
			Assert.False(user.IsValid);
			user.Id = "asom";
			Assert.False(user.IsValid);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("    ")]
		[InlineData("\n\t")]
		public void IsFalseIfPasswordHashInvalid(string hash)
		{
			var user = new User
			{
				UserName = "username",
				PasswordHash = hash
			};
			Assert.False(user.IsValid);
			user.Id = "asom";
			Assert.False(user.IsValid);
		}

		[Fact]
		public void IsTrueIfUserNameAndPasswordHashPresent()
		{
			var user = new User
			{
				UserName = "username",
				PasswordHash = "somehash"
			};
			Assert.True(user.IsValid);
			user.Id = "asom";
			Assert.True(user.IsValid);
		}
	}
}