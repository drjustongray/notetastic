using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Location_Equals
	{
		private readonly List<Location> list1 = new List<Location>();
		private readonly List<Location> list2 = new List<Location>();

		public Location_Equals()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var nbid in new string[] { null, "nbid1", "nbid2" })
						foreach (var archived in new bool[] { true, false })
							foreach (var title in new string[] { null, "sometitle", "some other title" })
								foreach (var lat in new double[] { 0, -93, 8.3 })
									foreach (var lon in new double[] { 0, 180.445, -173.2 })
									{
										list1.Add(new Location
										{
											Id = id,
											UID = uid,
											NBID = nbid,
											Archived = archived,
											Title = title,
											Latitude = lat,
											Longitude = lon
										});
										list2.Add(new Location
										{
											Id = id,
											UID = uid,
											NBID = nbid,
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
			var user = new Location();
			Assert.False(user.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var location in list1)
			{
				Assert.True(location.Equals(location));
			}
		}

		[Fact]
		public void ReturnsTrueIfIdentical()
		{
			for (int i = 0; i < list1.Count; i++)
			{
				Assert.True(list1[i].Equals(list2[i]));
			}
		}

		[Fact]
		public void ReturnsFalseIfDifferent()
		{
			for (int i = 0; i < list1.Count; i++)
				for (int j = 0; j < list1.Count; j++)
					if (i != j)
						Assert.False(list1[i].Equals(list2[j]));
		}
	}
}