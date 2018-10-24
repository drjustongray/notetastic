using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Notebook_Equals
	{
		private readonly List<Notebook> list1 = new List<Notebook>();
		private readonly List<Notebook> list2 = new List<Notebook>();

		public Notebook_Equals()
		{
			foreach (var isRoot in new bool[] { true, false })
				foreach (var id in new string[] { null, "someid", "someotherid" })
					foreach (var uid in new string[] { null, "uid1", "uid2" })
						foreach (var nbid in new string[] { null, "nbid1", "nbid2" })
							foreach (var archived in new bool[] { true, false })
								foreach (var title in new string[] { null, "sometitle", "some other title" })
									foreach (var items in new List<NotebookItem>[] {
									null,
									new List<NotebookItem> { },
									new List<NotebookItem> { new NotebookItem {Id = "somethig", Type="somethingesd", Title = "jfasdouf"} },
									new List<NotebookItem> { new NotebookItem {Id = "somig", Type="sometgesd", Title = "sdouf"}, new NotebookItem { Id = "somsdfgsig", Type = "somgsdfgetgesd", Title = "sdsdfgouf" } }
								})
									{
										list1.Add(new Notebook
										{
											Id = id,
											UID = uid,
											NBID = nbid,
											Archived = archived,
											Title = title,
											Items = items,
											IsRoot = isRoot
										});
										list2.Add(new Notebook
										{
											Id = id,
											UID = uid,
											NBID = nbid,
											Archived = archived,
											Title = title,
											Items = items != null ? new List<NotebookItem>(items) : null,
											IsRoot = isRoot
										});
									}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var notebook = new Notebook();
			Assert.False(notebook.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var notebook in list1)
			{
				Assert.True(notebook.Equals(notebook));
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
						Assert.False(list1[i].Equals(list2[j]), $"{list1[i].Items} : {list2[j].Items}");
		}
	}
}