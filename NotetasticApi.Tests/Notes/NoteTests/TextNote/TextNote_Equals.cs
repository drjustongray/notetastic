using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class TextNote_Equals
	{
		private readonly List<TextNote> list1 = new List<TextNote>();
		private readonly List<TextNote> list2 = new List<TextNote>();

		public TextNote_Equals()
		{
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var archived in new bool[] { true, false })
						foreach (var title in new string[] { null, "sometitle", "some other title" })
							foreach (var text in new string[] { null, "some super duper fun stuff", "some boring work stuff" })
							{
								list1.Add(new TextNote
								{
									Id = id,
									UID = uid,
									Archived = archived,
									Title = title,
									Text = text
								});
								list2.Add(new TextNote
								{
									Id = id,
									UID = uid,
									Archived = archived,
									Title = title,
									Text = text
								});
							}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var textnote = new TextNote();
			Assert.False(textnote.Equals(null));
		}

		[Fact]
		public void ReturnsTrueIfSameInstance()
		{
			foreach (var textnote in list1)
			{
				Assert.True(textnote.Equals(textnote));
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