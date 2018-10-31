using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Notes;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteControllerTests
{
	public class NoteController_PutNote : NoteController_Base
	{
		[Fact]
		public async void BadRequestIfArgNull()
		{
			var result = await noteController.PutNote(null);
			Assert.NotNull(result);
			Assert.IsType<BadRequestResult>(result.Result);
		}

		[Fact]
		public async void BadRequestIfInvalid()
		{
			var uid = "uid";
			var note = new TextNote { UID = uid };
			SetupUser(uid);
			validationService.Setup(x => x.IsNoteValid(note)).Returns(false);
			var result = await noteController.PutNote(note);
			Assert.NotNull(result);
			Assert.IsType<BadRequestResult>(result.Result);
		}

		[Fact]
		public async void CreatesNoteIfIdNull()
		{
			var note = new TextNote
			{
				Text = "some text",
				Title = "some title"
			};
			var uid = "uid1";
			SetupUser(uid);
			var toCreate = new TextNote
			{
				UID = uid,
				Text = "some text",
				Title = "some title"
			};
			validationService.Setup(x => x.IsNoteValid(toCreate)).Returns(true);
			var expected = new TextNote
			{
				Id = "someid1",
				UID = uid,
				Text = "some text",
				Title = "some title"
			};
			noteRepository.Setup(x => x.Create(toCreate)).ReturnsAsync(expected);

			var result = await noteController.PutNote(note);
			noteRepository.Verify(x => x.Create(toCreate));
			Assert.Equal(expected, result.Value);
		}

		[Fact]
		public async void UpdatesNoteIfIdNotNull()
		{
			var note = new Location
			{
				Id = "someid2",
				UID = "somesuch",
				Title = "some title",
				Longitude = 98,
				Latitude = 72
			};
			var uid = "uid2";
			SetupUser(uid);
			var expected = new Location
			{
				Id = "someid2",
				UID = uid,
				Title = "some title",
				Longitude = 98,
				Latitude = 72
			};

			validationService.Setup(x => x.IsNoteValid(expected)).Returns(true);
			noteRepository.Setup(x => x.Update(expected)).ReturnsAsync(expected);

			var result = await noteController.PutNote(note);
			Assert.Equal(expected, result.Value);
		}

		[Fact]
		public async void ReturnsNotFoundIfUpdateFails()
		{
			var note = new Bookmark
			{
				Id = "someid3",
				UID = "somesuch2",
				Title = "some title",
				URL = "someurl"
			};
			var uid = "uid3";
			SetupUser(uid);
			var expected = new Bookmark
			{
				Id = "someid3",
				UID = uid,
				Title = "some title",
				URL = "someurl"
			};

			validationService.Setup(x => x.IsNoteValid(expected)).Returns(true);
			noteRepository.Setup(x => x.Update(expected)).ReturnsAsync((Note)null);

			var result = await noteController.PutNote(note);
			Assert.NotNull(result);
			Assert.IsType<NotFoundResult>(result.Result);
		}
	}
}