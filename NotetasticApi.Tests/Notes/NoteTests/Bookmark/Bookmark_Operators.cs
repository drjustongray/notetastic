using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Bookmark_Operators
	{
		private readonly List<Bookmark> list1 = new List<Bookmark>();
		private readonly List<Bookmark> list2 = new List<Bookmark>();

		public Bookmark_Operators()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var archived in new bool[] { true, false })
						foreach (var title in new string[] { null, "sometitle", "some other title" })
							foreach (var url in new string[] { null, "http://somethingcool.com", "someotherurl" })
							{
								list1.Add(new Bookmark
								{
									Id = id,
									UID = uid,
									Archived = archived,
									Title = title,
									URL = url
								});
								list2.Add(new Bookmark
								{
									Id = id,
									UID = uid,
									Archived = archived,
									Title = title,
									URL = url
								});
							}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var bookmark = new Bookmark();
			Assert.False(bookmark == null);
			Assert.False(null == bookmark);
			Assert.True(bookmark != null);
			Assert.True(null != bookmark);
			bookmark = null;
			Assert.True(bookmark == null);
			Assert.True(null == bookmark);
			Assert.False(bookmark != null);
			Assert.False(null != bookmark);
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