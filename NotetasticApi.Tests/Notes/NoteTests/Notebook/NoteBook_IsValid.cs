using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class NoteBook_IsValid
	{
		[Theory]
		[InlineData(null, "title1", 0, true)]
		[InlineData("uid2", null, 9, false)]
		[InlineData("uid3", "title3", -10, true)]
		[InlineData("uid4", "title4", 8, null)]
		public void IsFalseIfARequiredPropertyNull(string uid, string title, int count, bool? archived)
		{
			Assert.False(new NoteBook { UID = uid, Title = title, Archived = archived, Count = count }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", 0, false)]
		[InlineData("uid2", "title2", 4, true)]
		[InlineData("uid3", "title3", 90, false)]
		public void IsTrueIfAllRequiredPropertiesGood(string uid, string title, int count, bool archived)
		{
			Assert.True(new NoteBook { UID = uid, Title = title, Archived = archived, Count = count }.IsValid);
		}
	}
}