using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Location_IsValid
	{
		[Theory]
		[InlineData(null, "title1", 0, -10)]
		[InlineData("uid2", null, 86, 180)]
		[InlineData("uid3", "title3", -91, -240)]
		[InlineData("uid4", "title4", 97, -10)]
		[InlineData("uid5", "title5", 86, 190)]
		public void IsFalseIfARequiredPropertyNullOrOutsideRange(string uid, string title, double lat, double lon)
		{
			Assert.False(new Location { UID = uid, Title = title, Latitude = lat, Longitude = lon }.IsValid);
		}

		[Theory]
		[InlineData("uid1", "title1", 0, 0)]
		[InlineData("uid2", "title2", 70, -130)]
		[InlineData("uid3", "title3", -8, 179)]
		public void IsTrueIfAllRequiredPropertiesPresent(string uid, string title, double lat, double lon)
		{
			Assert.True(new Location { UID = uid, Title = title, Latitude = lat, Longitude = lon }.IsValid);
		}
	}
}