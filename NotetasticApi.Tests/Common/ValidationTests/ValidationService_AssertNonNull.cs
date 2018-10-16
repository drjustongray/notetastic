using System;
using NotetasticApi.Common;
using Xunit;

namespace NotetasticApi.Tests.Common.ValidationTests
{
	public class ValidationService_AssertNonNull
	{
		private ValidationService _service = new ValidationService();

		[Theory]
		[InlineData(1)]
		[InlineData("fasd")]
		public void DoesNotThrowIfNonNullValue(object o)
		{
			_service.AssertNonNull(o, "");
		}

		[Theory]
		[InlineData("humbug")]
		[InlineData("greg")]
		[InlineData("")]
		public void ThrowsIfNullValue(string paramName)
		{
			var exception = Assert.Throws<ArgumentNullException>(
				() => _service.AssertNonNull(null, paramName)
			);
			Assert.Equal(paramName, exception.ParamName);
		}
	}
}