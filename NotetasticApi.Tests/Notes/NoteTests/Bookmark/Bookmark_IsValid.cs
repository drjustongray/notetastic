using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Bookmark_IsValid
	{
		[Theory]
		[InlineData(null, "title1", "url1", true)]
		[InlineData("uid2", null, "url2", false)]
		[InlineData("uid3", "title3", null, true)]
		[InlineData("uid4", "title4", "url4", null)]
		public void IsFalseIfARequiredPropertyNull(string uid, string title, string url, bool? archived)
		{
			Assert.False(new Bookmark { UID = uid, Title = title, URL = url, Archived = archived }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", "url1", true)]
		[InlineData("uid2", "title2", "url2", false)]
		[InlineData("uid3", "title3", "url3", true)]
		public void IsTrueIfAllRequiredPropertiesPresent(string uid, string title, string url, bool archived)
		{
			Assert.True(new Bookmark { UID = uid, Title = title, URL = url, Archived = archived }.IsValid);
		}
	}
}