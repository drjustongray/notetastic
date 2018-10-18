using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class TextNote_IsValid
	{
		[Theory]
		[InlineData(null, "title1", "some really cool text ", true)]
		[InlineData("uid2", null, "some more really cool text", true)]
		[InlineData("uid3", "title3", null, true)]
		[InlineData("uid3", "title3", "", null)]
		public void IsFalseIfARequiredPropertyNull(string uid, string title, string text, bool? archived)
		{
			Assert.False(new TextNote { UID = uid, Title = title, Archived = archived, Text = text }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", "some really cool text ", false)]
		[InlineData("uid2", "title2", "some more really cool text", true)]
		[InlineData("uid3", "title3", "", false)]
		public void IsTrueIfAllRequiredPropertiesPresent(string uid, string title, string text, bool archived)
		{
			Assert.True(new TextNote { UID = uid, Title = title, Archived = archived, Text = text }.IsValid);
		}
	}
}