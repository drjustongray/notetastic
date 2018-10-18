using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Bookmark_Equals
	{
		private readonly List<Bookmark> list1 = new List<Bookmark>();
		private readonly List<Bookmark> list2 = new List<Bookmark>();

		public Bookmark_Equals()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var nbid in new string[] { null, "nbid1", "nbid2" })
						foreach (var archived in new bool[] { true, false })
							foreach (var title in new string[] { null, "sometitle", "some other title" })
								foreach (var url in new string[] { null, "http://somethingcool.com", "someotherurl" })
								{
									list1.Add(new Bookmark
									{
										Id = id,
										UID = uid,
										NBID = nbid,
										Archived = archived,
										Title = title,
										URL = url
									});
									list2.Add(new Bookmark
									{
										Id = id,
										UID = uid,
										NBID = nbid,
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
			Assert.False(bookmark.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var bookmark in list1)
			{
				Assert.True(bookmark.Equals(bookmark));
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