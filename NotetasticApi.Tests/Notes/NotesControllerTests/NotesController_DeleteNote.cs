using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteControllerTests
{
	public class NotesController_DeleteNote : NotesController_Base
	{
		[Theory]
		[InlineData("id1", "uid1")]
		[InlineData("id2", "uid2")]
		[InlineData("id3", "uid3")]
		[InlineData("id4", "uid4")]
		public async void ReturnsOkIfSuccess(string id, string uid)
		{
			noteRepository.Setup(x => x.Delete(id, uid)).ReturnsAsync(true);

			SetupUser(uid);
			var result = await noteController.DeleteNote(id);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<OkResult>(result);
		}

		[Theory]
		[InlineData("id1", "uid1")]
		[InlineData("id2", "uid2")]
		[InlineData("id3", "uid3")]
		[InlineData("id4", "uid4")]
		public async void ReturnsNotFoundIfFailed(string id, string uid)
		{
			noteRepository.Setup(x => x.Delete(id, uid)).ReturnsAsync(false);

			SetupUser(uid);
			var result = await noteController.DeleteNote(id);

			Assert.NotNull(result);
			Assert.IsAssignableFrom<NotFoundResult>(result);
		}
	}
}