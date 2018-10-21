using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class NoteBook_Operators
	{
		private readonly List<NoteBook> list1 = new List<NoteBook>();
		private readonly List<NoteBook> list2 = new List<NoteBook>();

		public NoteBook_Operators()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var nbid in new string[] { null, "nbid1", "nbid2" })
						foreach (var archived in new bool[] { true, false })
							foreach (var title in new string[] { null, "sometitle", "some other title" })
								foreach (var items in new List<NoteBookItem>[] {
									null,
									new List<NoteBookItem> { },
									new List<NoteBookItem> { new NoteBookItem {Id = "somethig", Type="somethingesd", Title = "jfasdouf"} },
									new List<NoteBookItem> { new NoteBookItem {Id = "somig", Type="sometgesd", Title = "sdouf"}, new NoteBookItem { Id = "somsdfgsig", Type = "somgsdfgetgesd", Title = "sdsdfgouf" } }
								})
								{
									list1.Add(new NoteBook
									{
										Id = id,
										UID = uid,
										NBID = nbid,
										Archived = archived,
										Title = title
									});
									list2.Add(new NoteBook
									{
										Id = id,
										UID = uid,
										NBID = nbid,
										Archived = archived,
										Title = title
									});
								}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var notebook = new NoteBook();
			Assert.False(notebook == null);
			Assert.False(null == notebook);
			Assert.True(notebook != null);
			Assert.True(null != notebook);
			notebook = null;
			Assert.True(notebook == null);
			Assert.True(null == notebook);
			Assert.False(notebook != null);
			Assert.False(null != notebook);
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