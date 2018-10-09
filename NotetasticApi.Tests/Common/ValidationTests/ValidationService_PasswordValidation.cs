using System;
using NotetasticApi.Common;
using Xunit;

namespace NotetasticApi.Tests.Common.ValidationTests
{
	public class ValidationService_PasswordValidation
	{
		private ValidationService _service = new ValidationService();

		[Fact]
		public void FailureWhenNull()
		{
			string reason;
			var actual = _service.IsPasswordValid(null, out reason);
			Assert.False(actual);
			Assert.Equal("Password is missing", reason);
			var exception = Assert.Throws<ArgumentException>(() => _service.ValidatePassword(null));
			Assert.Equal(reason, exception.Message);
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("s df")]
		[InlineData("\tfas\ndf")]

		public void FailureWhenThereIsWhitespace(string password)
		{
			string reason;
			var actual = _service.IsPasswordValid(password, out reason);
			Assert.False(actual);
			Assert.Equal("Password must not contain white space", reason);
			var exception = Assert.Throws<ArgumentException>(() => _service.ValidatePassword(password));
			Assert.Equal(reason, exception.Message);
		}

		[Theory]
		[InlineData("")]
		[InlineData("asdf")]
		[InlineData("ass")]
		[InlineData("hjkdas")]
		[InlineData("dsafsdj")]
		[InlineData("f")]
		[InlineData("✅🐎🔋🖇123")]
		public void FailureWhenTooShort(string password)
		{
			string reason;
			var actual = _service.IsPasswordValid(password, out reason);
			Assert.False(actual);
			Assert.Equal("Password must be at least 8 characters long", reason);
			var exception = Assert.Throws<ArgumentException>(() => _service.ValidatePassword(password));
			Assert.Equal(reason, exception.Message);
		}

		[Theory]
		[InlineData("12345678")]
		[InlineData("password")]
		[InlineData("asdfwerw3")]
		[InlineData("fasdf46asd5f")]
		public void SuccessWhenAllConditionsMet(string password)
		{
			string reason;
			var actual = _service.IsPasswordValid(password, out reason);
			Assert.True(actual);
			Assert.Equal("", reason);
			_service.ValidatePassword(password);
		}
	}
}