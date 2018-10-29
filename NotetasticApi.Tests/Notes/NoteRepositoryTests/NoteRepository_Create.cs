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
				() => repo.Create(new Bookmark())
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ThrowsIfNoteHasId()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => repo.Create(new Bookmark { Id = "asdf", Title = "title", UID = "uid", Archived = false, URL = "someurl" })
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1", "title1", "url1")]
		[InlineData("uid2", "title2", "url2")]
		public async void CreatesAndReturnsNote(string uid, string title, string url)
		{
			var note = new Bookmark { UID = uid, Title = title, URL = url, Archived = false };
			var returnedNote = await repo.Create(note);

			Assert.NotNull(returnedNote);
			Assert.NotNull(returnedNote.Id);
			note.Id = returnedNote.Id;

			Assert.Equal(note, returnedNote);

			var expected = expectedCollection;
			expected.Add(note);

			AssertCollectionEquals(expected);
		}

		[Theory]
		[InlineData("uid1", "title1", -1, null)]
		[InlineData("uid1", "title2", 4, true)]
		public async void SetsArchivedToFalse(string uid, string title, int count, bool? archived)
		{

			var items = new List<CheckItem>();
			for (int i = 0; i < count; i++)
			{
				items.Add(new CheckItem { Checked = i % 2 == 0, Text = title + i });
			}

			var note = new Checklist { UID = uid, Title = title, Items = items, Archived = archived };
			var expected = new Checklist { UID = uid, Title = title, Items = items, Archived = false };

			var actual = await repo.Create(note);
			expected.Id = actual.Id;
			Assert.Equal(expected, actual);
			var set = expectedCollection;
			set.Add(expected);
			AssertCollectionEquals(set);
		}
	}
}