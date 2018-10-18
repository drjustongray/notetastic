using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Location_IsValid
	{
		[Theory]
		[InlineData(null, "title1", 0, -10, true)]
		[InlineData("uid2", null, 86, 180, false)]
		[InlineData("uid3", "title3", -91, -240, true)]
		[InlineData("uid4", "title4", 97, -10, false)]
		[InlineData("uid5", "title5", 86, 190, true)]
		[InlineData("uid5", "title5", 86, 7, null)]
		public void IsFalseIfARequiredPropertyNullOrOutsideRange(string uid, string title, double lat, double lon, bool? archived)
		{
			Assert.False(new Location { UID = uid, Title = title, Archived = archived, Latitude = lat, Longitude = lon }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", 0, 0, true)]
		[InlineData("uid2", "title2", 70, -130, false)]
		[InlineData("uid3", "title3", -8, 179, true)]
		public void IsTrueIfAllRequiredPropertiesPresent(string uid, string title, double lat, double lon, bool archived)
		{
			Assert.True(new Location { UID = uid, Title = title, Archived = archived, Latitude = lat, Longitude = lon }.IsValid);
		}
	}
}