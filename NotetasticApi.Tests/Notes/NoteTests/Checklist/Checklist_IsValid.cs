using System.Collections.Generic;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteTests
{
	public class Checklist_IsValid
	{
		[Theory]
		[InlineData(null, "Title", true)]
		[InlineData("Uid", null, false)]
		[InlineData("uid", "title", null)]
		public void IsFalseIfUidTitleOrArchivedNull(string uid, string title, bool? archived)
		{
			Assert.False(new Checklist { UID = uid, Title = title, Archived = archived, Items = new List<CheckItem>() }.IsValid);
		}

		[Fact]
		public void IsFalseIfItemsNull()
		{
			Assert.False(new Checklist { UID = "uid", Title = "title", Archived = false }.IsValid);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(3)]
		[InlineData(5)]
		public void IsFalseIfACheckItemMissingText(int index)
		{
			var items = new List<CheckItem>();
			for (int i = 0; i < 10; i++)
			{
				var item = new CheckItem { Checked = i % 2 == 0, Text = i == index ? null : "sometext" + i };
				items.Add(item);
			}
			Assert.False(new Checklist { UID = "uid", Title = "title", Archived = true, Items = items }.IsValid);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(3)]
		[InlineData(5)]
		public void IsTrueIfAllRequiredPropsPresentAndCheckItemsGood(int length)
		{
			var items = new List<CheckItem>();
			for (int i = 0; i < length; i++)
			{
				var item = new CheckItem { Checked = i % 2 == 0, Text = "sometext" + i };
				items.Add(item);
			}
			Assert.True(new Checklist { UID = "uid", Title = "title", Archived = false, Items = items }.IsValid);
		}

	}
}