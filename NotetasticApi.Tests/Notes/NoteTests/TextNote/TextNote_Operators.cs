using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class TextNote_Operators
	{
		private readonly List<TextNote> list1 = new List<TextNote>();
		private readonly List<TextNote> list2 = new List<TextNote>();

		public TextNote_Operators()
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
			Assert.False(textnote == null);
			Assert.False(null == textnote);
			Assert.True(textnote != null);
			Assert.True(null != textnote);
			textnote = null;
			Assert.True(textnote == null);
			Assert.True(null == textnote);
			Assert.False(textnote != null);
			Assert.False(null != textnote);
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