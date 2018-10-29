using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Location_Operators
	{
		private readonly List<Location> list1 = new List<Location>();
		private readonly List<Location> list2 = new List<Location>();

		public Location_Operators()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var archived in new bool[] { true, false })
						foreach (var title in new string[] { null, "sometitle", "some other title" })
							foreach (var lat in new double[] { 0, -93, 8.3 })
								foreach (var lon in new double[] { 0, 180.445, -173.2 })
								{
									list1.Add(new Location
									{
										Id = id,
										UID = uid,
										Archived = archived,
										Title = title,
										Latitude = lat,
										Longitude = lon
									});
									list2.Add(new Location
									{
										Id = id,
										UID = uid,
										Archived = archived,
										Title = title,
										Latitude = lat,
										Longitude = lon
									});
								}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var location = new Location();
			Assert.False(location == null);
			Assert.False(null == location);
			Assert.True(location != null);
			Assert.True(null != location);
			location = null;
			Assert.True(location == null);
			Assert.True(null == location);
			Assert.False(location != null);
			Assert.False(null != location);
		}

		[Fact]
		public void AgreesWithEquals()
		{
			for (int i = 0; i < list1.Count; i++)
				for (int j = 0; j < list1.Count; j++)
				{
					var u1 = list1[i];
					var u2 = list2[j];
					Assert.Equal(u1.Equals(u2), u1 == u2);
					Assert.Equal(!u1.Equals(u2), u1 != u2);
				}
		}
	}
}