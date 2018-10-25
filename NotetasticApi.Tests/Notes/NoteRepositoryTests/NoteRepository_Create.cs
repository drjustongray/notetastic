using System;
using System.Collections.Generic;
using MongoDB.Bson;
using NotetasticApi.Common;
using NotetasticApi.Notes;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	public class NoteRepository_Create : NoteRepository_Base
	{
		public NoteRepository_Create(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ThrowsIfArgNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => repo.Create(null)
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ThrowsIfNoteInvalid()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => repo.Create(new Notebook())
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ThrowsIfNoteHasId()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => repo.Create(new Notebook { Id = "asdf", Title = "title", UID = "uid", Archived = false })
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1", "title1", "url1")]
		[InlineData("uid2", "title2", "url2")]
		public async void AddsNoteToRootIfNBIDNull(string uid, string title, string url)
		{
			var note = new Bookmark { UID = uid, Title = title, URL = url, Archived = false };
			var returnedNote = await repo.Create(note);

			Assert.NotNull(returnedNote);
			Assert.NotNull(returnedNote.Id);
			note.Id = returnedNote.Id;


			var root = uid == "uid1" ? root1 : root2;
			Assert.Equal(root.Id, returnedNote.NBID);
			note.NBID = returnedNote.NBID;
			Assert.Equal(note, returnedNote);

			root.Items.Add(new NotebookItem { Id = note.Id, Title = title, Type = "Bookmark" });
			var expected = expectedCollection;
			expected.Add(note);

			AssertCollectionEquals(expected);
		}

		[Theory]
		[InlineData("uid3", "title1", 89, 72)]
		[InlineData("uid4", "title2", .4, 56)]
		public async void CreatesRootIfNecessary(string uid, string title, double lat, double lon)
		{
			var location = new Location { UID = uid, Title = title, Archived = false, Latitude = lat, Longitude = lon };
			var returnedNote = await repo.Create(location);
			Assert.NotNull(returnedNote.NBID);
			var shouldBeRoot = await collection.FindOneAsync(new BsonDocument("_id", returnedNote.NBID));
			var expectedRoot = new Notebook
			{
				Id = shouldBeRoot.Id,
				UID = uid,
				IsRoot = true,
				Items = new List<NotebookItem> { new NotebookItem { Id = returnedNote.Id, Type = "Location", Title = title } }
			};
			Assert.Equal(expectedRoot, shouldBeRoot);
		}

		[Theory]
		[InlineData("uid1", "title1", "some text 1", 1)]
		[InlineData("uid1", "title2", "some text 2", 2)]
		[InlineData("uid2", "title3", "some text 3", 3)]
		public async void AddsNoteToGivenNotebook(string uid, string title, string text, int nb)
		{
			Notebook notebook = null;
			switch (nb)
			{
				case 1:
					notebook = notebook1;
					break;
				case 2:
					notebook = notebook2;
					break;
				case 3:
					notebook = notebook3;
					break;
			}

			var note = new TextNote { UID = uid, Title = title, Text = text, Archived = false, NBID = notebook.Id };
			var returnedNote = await repo.Create(note);

			Assert.NotNull(returnedNote);
			Assert.NotNull(returnedNote.Id);
			note.Id = returnedNote.Id;

			Assert.Equal(note, returnedNote);

			notebook.Items.Add(new NotebookItem { Id = note.Id, Title = title, Type = "TextNote" });
			var expected = expectedCollection;
			expected.Add(note);

			AssertCollectionEquals(expected);
		}

		[Theory]
		[InlineData("uid1", "title1", "text 1", 1)]
		[InlineData("uid1", "title2", "text 2", 2)]
		[InlineData("uid2", "title3", "text 3", 3)]

		[InlineData("uid2", "title3", "text 3", 4)]
		public async void ReturnsNullDoesNothingIfNotebookNotExistOrNotTheirs(string uid, string title, string text, int nb)
		{
			Notebook notebook = null;
			switch (nb)
			{
				case 1:
					notebook = new Notebook { Id = "someid" };
					break;
				case 2:
					notebook = notebook3;
					break;
				case 3:
					notebook = notebook2;
					break;
				case 4:
					notebook = new Notebook { Id = note2.Id };
					break;
			}

			var note = new TextNote { UID = uid, Title = title, Text = text, Archived = false, NBID = notebook.Id };
			var returnedNote = await repo.Create(note);

			Assert.Null(returnedNote);

			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1", "title1", -1, 1)]
		[InlineData("uid1", "title2", 4, 2)]
		[InlineData("uid2", "title3", 0, 3)]
		public async void SavesNotebooksWithEmptyItemsListAlways(string uid, string title, int count, int nb)
		{
			Notebook notebook = null;
			switch (nb)
			{
				case 1:
					notebook = notebook1;
					break;
				case 2:
					notebook = notebook2;
					break;
				case 3:
					notebook = notebook3;
					break;
			}

			var expected = new Notebook { UID = uid, Title = title, Items = new List<NotebookItem>(), Archived = false, NBID = notebook.Id };
			var items = count != -1 ? new List<NotebookItem>() : null;
			for (int i = 0; i < count; i++)
			{
				items.Add(new NotebookItem { Id = i.ToString(), Title = title + i, Type = "Bookmark" });
			}

			var note = new Notebook { UID = uid, Title = title, Items = items, Archived = false, NBID = notebook.Id };

			var actual = await repo.Create(note);
			expected.Id = actual.Id;
			Assert.Equal(expected, actual);
			notebook.Items.Add(new NotebookItem { Id = actual.Id, Title = title, Type = "Notebook" });
			var set = expectedCollection;
			set.Add(expected);
			AssertCollectionEquals(set);
		}

		[Theory]
		[InlineData("uid1", "title1", -1, 1, null)]
		[InlineData("uid1", "title2", 4, 2, true)]
		public async void SetsArchivedToFalse(string uid, string title, int count, int nb, bool? archived)
		{
			Notebook notebook = null;
			switch (nb)
			{
				case 1:
					notebook = notebook1;
					break;
				case 2:
					notebook = notebook2;
					break;
				case 3:
					notebook = notebook3;
					break;
			}

			var items = new List<CheckItem>();
			for (int i = 0; i < count; i++)
			{
				items.Add(new CheckItem { Checked = i % 2 == 0, Text = title + i });
			}

			var note = new Checklist { UID = uid, Title = title, Items = items, Archived = archived, NBID = notebook.Id };
			var expected = new Checklist { UID = uid, Title = title, Items = items, Archived = false, NBID = notebook.Id };

			var actual = await repo.Create(note);
			expected.Id = actual.Id;
			Assert.Equal(expected, actual);
			notebook.Items.Add(new NotebookItem { Id = actual.Id, Title = title, Type = "Checklist" });
			var set = expectedCollection;
			set.Add(expected);
			AssertCollectionEquals(set);
		}
	}
}