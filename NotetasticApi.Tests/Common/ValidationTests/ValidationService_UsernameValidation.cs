using System;
using NotetasticApi.Common;
using Xunit;

namespace NotetasticApi.Tests.Common.ValidationTests
{
	public class ValidationService_UsernameValidation
	{
		private ValidationService _service = new ValidationService();

		[Fact]
		public void FailureWhenNull()
		{
			string reason;
			var actual = _service.IsUsernameValid(null, out reason);
			Assert.False(actual);
			Assert.Equal("Username is missing", reason);
			var exception = Assert.Throws<ArgumentException>(() => _service.ValidateUsername(null));
			Assert.Equal(reason, exception.Message);
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("s df")]
		[InlineData("\tfas\ndf")]

		public void FailureWhenThereIsWhitespace(string username)
		{
			string reason;
			var actual = _service.IsUsernameValid(username, out reason);
			Assert.False(actual);
			Assert.Equal("Username must not contain white space", reason);
			var exception = Assert.Throws<ArgumentException>(() => _service.ValidateUsername(username));
			Assert.Equal(reason, exception.Message);
		}

		[Theory]
		[InlineData("")]
		[InlineData("a")]
		[InlineData("as")]
		[InlineData("✅")]
		[InlineData("✅🐎")]
		public void FailureWhenTooShort(string username)
		{
			string reason;
			var actual = _service.IsUsernameValid(username, out reason);
			Assert.False(actual);
			Assert.Equal("Username must be at least 3 characters long", reason);
			var exception = Assert.Throws<ArgumentException>(() => _service.ValidateUsername(username));
			Assert.Equal(reason, exception.Message);
		}

		[Theory]
		[InlineData("user")]
		[InlineData("asd")]
		[InlineData("✅🐎🐎")]
		[InlineData("fasdf46asd5f")]
		public void SuccessWhenAllConditionsMet(string username)
		{
			string reason;
			var actual = _service.IsUsernameValid(username, out reason);
			Assert.True(actual);
			Assert.Equal("", reason);
			_service.ValidateUsername(username);
		}
	}
}