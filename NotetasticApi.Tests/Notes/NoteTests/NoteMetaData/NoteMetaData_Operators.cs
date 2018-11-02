using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class NoteMetaData_Operators
	{
		private readonly List<NoteMetaData> list1 = new List<NoteMetaData>();
		private readonly List<NoteMetaData> list2 = new List<NoteMetaData>();

		public NoteMetaData_Operators()
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
			Assert.False(noteMetaData == null);
			Assert.False(null == noteMetaData);
			Assert.True(noteMetaData != null);
			Assert.True(null != noteMetaData);
			noteMetaData = null;
			Assert.True(noteMetaData == null);
			Assert.True(null == noteMetaData);
			Assert.False(noteMetaData != null);
			Assert.False(null != noteMetaData);
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