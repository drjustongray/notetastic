using System;
using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Checklist_Operators
	{
		private readonly List<Checklist> list1 = new List<Checklist>();
		private readonly List<Checklist> list2 = new List<Checklist>();

		public Checklist_Operators()
		{
			var rand = new Random();
			foreach (var id in new string[] { null, "someid", "someotherid" })
				foreach (var uid in new string[] { null, "uid1", "uid2" })
					foreach (var archived in new bool[] { true, false })
						foreach (var title in new string[] { null, "sometitle", "some other title" })
							foreach (var itemCounts in new int[] { -1, 0, 3, 3, 5, 5, 10, 10 })
							{
								var items1 = itemCounts != -1 ? new List<CheckItem>() : null;
								var items2 = itemCounts != -1 ? new List<CheckItem>() : null;
								for (int i = 0; i < itemCounts; i++)
								{
									var item = new CheckItem
									{
										Checked = rand.Next() % 2 == 0,
										Text = rand.Next() % 10 == 0 ? null : "sometext" + rand.Next()
									};
									items1.Add(item);
									items2.Add(item);
								}
								list1.Add(new Checklist
								{
									Id = id,
									UID = uid,
									Archived = archived,
									Title = title,
									Items = items1
								});
								list2.Add(new Checklist
								{
									Id = id,
									UID = uid,
									Archived = archived,
									Title = title,
									Items = items2
								});
							}
		}

		[Fact]
		public void HandlesNullCorrectly()
		{
			var checklist = new Checklist();
			Assert.False(checklist == null);
			Assert.False(null == checklist);
			Assert.True(checklist != null);
			Assert.True(null != checklist);
			checklist = null;
			Assert.True(checklist == null);
			Assert.True(null == checklist);
			Assert.False(checklist != null);
			Assert.False(null != checklist);
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