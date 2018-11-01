using System.Collections.Generic;
using Moq;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteControllerTests
{
	public class NotesController_GetNoteList : NotesController_Base
	{
		[Theory]
		[InlineData("uid1", 0)]
		[InlineData("uid2", 1)]
		[InlineData("uid3", 3)]
		[InlineData("uid4", 5)]
		public async void ReturnsList(string uid, int count)
		{
			var notes = new List<NoteMetaData>();
			for (int i = 0; i < count; i++)
			{
				notes.Add(new NoteMetaData
				{
					Id = "someid" + i,
					Title = "sometitle" + i,
					Type = "sometype" + i
				});
			}
			noteRepository.Setup(x => x.FindAllByUser(uid)).ReturnsAsync(notes);

			SetupUser(uid);
			var result = await noteController.GetNoteList();

			Assert.NotNull(result);
			Assert.Equal(notes as IEnumerable<NoteMetaData>, result.Value);
		}
	}
}