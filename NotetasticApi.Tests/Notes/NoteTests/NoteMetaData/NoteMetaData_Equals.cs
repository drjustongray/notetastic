using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class NoteMetaData_Equals
	{
		private readonly List<NoteMetaData> list1 = new List<NoteMetaData>();
		private readonly List<NoteMetaData> list2 = new List<NoteMetaData>();

		public NoteMetaData_Equals()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var title in new string[] { null, "sometitle", "some other title" })
					foreach (var type in new string[] { "Type1", "Type2", "Type3" })
					{
						list1.Add(new NoteMetaData
						{
							Id = id,
							Title = title,
							Type = type
						});
						list2.Add(new NoteMetaData
						{
							Id = id,
							Title = title,
							Type = type
						});
					}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var noteMetaData = new NoteMetaData();
			Assert.False(noteMetaData.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var noteMetaData in list1)
			{
				Assert.True(noteMetaData.Equals(noteMetaData));
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