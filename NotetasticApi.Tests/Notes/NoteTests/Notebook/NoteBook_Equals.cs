using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class NoteBook_Equals
	{
		private readonly List<NoteBook> list1 = new List<NoteBook>();
		private readonly List<NoteBook> list2 = new List<NoteBook>();

		public NoteBook_Equals()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var nbid in new string[] { null, "nbid1", "nbid2" })
						foreach (var archived in new bool[] { true, false })
							foreach (var title in new string[] { null, "sometitle", "some other title" })
								foreach (var count in new int[] { 0, 1, 10, -3 })
								{
									list1.Add(new NoteBook
									{
										Id = id,
										UID = uid,
										NBID = nbid,
										Archived = archived,
										Title = title,
										Count = count
									});
									list2.Add(new NoteBook
									{
										Id = id,
										UID = uid,
										NBID = nbid,
										Archived = archived,
										Title = title,
										Count = count
									});
								}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var notebook = new NoteBook();
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
						Assert.False(list1[i].Equals(list2[j]));
		}
	}
}