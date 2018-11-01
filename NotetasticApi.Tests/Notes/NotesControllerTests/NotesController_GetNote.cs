using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteControllerTests
{
	public class NotesController_GetNote : NotesController_Base
	{
		[Theory]
		[InlineData("id1", "uid1")]
		[InlineData("id2", "uid2")]
		[InlineData("id3", "uid3")]
		[InlineData("id4", "uid4")]
		public async void ReturnsNoteIfFound(string id, string uid)
		{
			var note = new Bookmark();
			noteRepository.Setup(x => x.FindById(id, uid)).ReturnsAsync(note);

			SetupUser(uid);
			var result = await noteController.GetNote(id);

			Assert.NotNull(result);
			Assert.Equal(note, result.Value);
		}

		[Theory]
		[InlineData("id1", "uid1")]
		[InlineData("id2", "uid2")]
		[InlineData("id3", "uid3")]
		[InlineData("id4", "uid4")]
		public async void ReturnsNotFoundIfNotFound(string id, string uid)
		{
			noteRepository.Setup(x => x.FindById(id, uid)).ReturnsAsync(null as Note);

			SetupUser(uid);
			var result = await noteController.GetNote(id);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<NotFoundResult>(result.Result);
		}
	}
}