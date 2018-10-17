using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class NoteBook_IsValid
	{
		[Theory]
		[InlineData(null, "title1", 0)]
		[InlineData("uid2", null, 9)]
		[InlineData("uid3", "title3", -10)]
		public void IsFalseIfARequiredPropertyNull(string uid, string title, int count)
		{
			Assert.False(new NoteBook { UID = uid, Title = title, Count = count }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", 0)]
		[InlineData("uid2", "title2", 4)]
		[InlineData("uid3", "title3", 90)]
		public void IsTrueIfAllRequiredPropertiesGood(string uid, string title, int count)
		{
			Assert.True(new NoteBook { UID = uid, Title = title, Count = count }.IsValid);
		}
	}
}