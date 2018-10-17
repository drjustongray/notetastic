using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Bookmark_IsValid
	{
		[Theory]
		[InlineData(null, "title1", "url1")]
		[InlineData("uid2", null, "url2")]
		[InlineData("uid3", "title3", null)]
		public void IsFalseIfARequiredPropertyNull(string uid, string title, string url)
		{
			Assert.False(new Bookmark { UID = uid, Title = title, URL = url }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", "url1")]
		[InlineData("uid2", "title2", "url2")]
		[InlineData("uid3", "title3", "url3")]
		public void IsTrueIfAllRequiredPropertiesPresent(string uid, string title, string url)
		{
			Assert.True(new Bookmark { UID = uid, Title = title, URL = url }.IsValid);
		}
	}
}