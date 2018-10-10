using Xunit;

namespace NotetasticApi.Tests.Users
{
	public class PasswordService
	{
		private NotetasticApi.Users.PasswordService _service = new NotetasticApi.Users.PasswordService();

		[Theory]
		[InlineData("sdfgsdfgsdfg")]
		[InlineData("fav43va vAWSEF")]
		[InlineData("34a4fa")]
		[InlineData("V ASD FQAWE4fv")]
		public void HashFunctionReternsBcryptHash(string password)
		{
			Assert.True(BCrypt.Net.BCrypt.Verify(password, _service.Hash(password)));
		}

		[Theory]
		[InlineData("sdfgsdfgsdfg")]
		[InlineData("fav43va vAWSEF")]
		[InlineData("34a4fa")]
		[InlineData("V ASD FQAWE4fv")]
		public void VerifyFunctionVerifiesMatch(string password)
		{
			Assert.True(_service.Verify(password, BCrypt.Net.BCrypt.HashPassword(password)));
		}

		[Theory]
		[InlineData("sdfgsdfgsdfg", "asdfkjasdlfkj")]
		[InlineData("fav43va vAWSEF", "fasd")]
		[InlineData("34a4fa", ":P")]
		[InlineData("V ASD FQAWE4fv", "asdfkja;sldkjf")]
		public void VerifyFunctionDoesNotVerifyNonMatch(string password, string wrong)
		{
			Assert.False(_service.Verify(wrong, BCrypt.Net.BCrypt.HashPassword(password)));
		}
	}
}