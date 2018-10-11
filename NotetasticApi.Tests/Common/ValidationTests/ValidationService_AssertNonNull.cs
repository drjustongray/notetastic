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

		[Fact]
		public void ThrowsIfNullValue()
		{
			var exception = Assert.Throws<ArgumentNullException>(
				() => _service.AssertNonNull<string>(null, "var1")
			);
			Assert.Equal("var1", exception.ParamName);
			exception = Assert.Throws<ArgumentNullException>(
				() => _service.AssertNonNull<object>(null, "var2")
			);
			Assert.Equal("var2", exception.ParamName);
			exception = Assert.Throws<ArgumentNullException>(
				() => _service.AssertNonNull<ValidationService>(null, "var3")
			);
			Assert.Equal("var3", exception.ParamName);
			exception = Assert.Throws<ArgumentNullException>(
				() => _service.AssertNonNull<Exception>(null, "var4")
			);
			Assert.Equal("var4", exception.ParamName);
		}
	}
}