using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Notebook_IsValid
	{
		[Theory]
		[InlineData(null, "title1", true)]
		[InlineData("uid2", null, false)]
		[InlineData("uid3", "title3", null)]
		public void IsFalseIfARequiredPropertyNull(string uid, string title, bool? archived)
		{
			Assert.False(new Notebook { UID = uid, Title = title, Archived = archived }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", false)]
		[InlineData("uid2", "title2", true)]
		[InlineData("uid3", "title3", false)]
		public void IsTrueIfAllRequiredPropertiesGood(string uid, string title, bool archived)
		{
			Assert.True(new Notebook { UID = uid, Title = title, Archived = archived }.IsValid);
		}
	}
}