using System;
using System.Collections.Generic;
using NotetasticApi.Notes;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	public class NoteRepository_Update : NoteRepository_Base
	{
		public NoteRepository_Update(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ThrowsIfArgNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => repo.Update(null)
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ThrowsIfNoteInvalid()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => repo.Update(new TextNote { Id = "asdf" })
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ThrowsIfNoteMissingId()
		{
			await Assert.ThrowsAsync<ArgumentException>(
				() => repo.Update(new TextNote { Title = "title", UID = "uid", Archived = false, Text = "" })
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void UpdatesNoteIfIdAndUIDMatchExisting(string uid)
		{
			var toUpdate = uid == "uid1" ? note1 : note2;
			var updated = new TextNote
			{
				Id = toUpdate.Id,
				UID = uid,
				Archived = !toUpdate.Archived,
				Title = "new title",
				Text = "new text"
			};

			var result = await repo.Update(updated);

			var expected = expectedCollection;
			expected.Remove(toUpdate);
			expected.Add(updated);

			Assert.Equal(updated, result);
			AssertCollectionEquals(expected);
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void DoesNothingIfUIDMisMatch(string uid)
		{
			var toUpdate = uid == "uid2" ? note1 : note2;
			var updated = new TextNote
			{
				Id = toUpdate.Id,
				UID = uid,
				Archived = !toUpdate.Archived,
				Title = "new title",
				Text = "new text"
			};

			var result = await repo.Update(updated);

			Assert.Null(result);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void DoesNothingIfDocumentNotExist(string uid)
		{
			var toUpdate = uid == "uid2" ? note1 : note2;
			var updated = new TextNote
			{
				Id = "randomid",
				UID = uid,
				Archived = !toUpdate.Archived,
				Title = "new title",
				Text = "new text"
			};

			var result = await repo.Update(updated);

			Assert.Null(result);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void DoesNothingIfTypeWrong(string uid)
		{
			var toUpdate = uid == "uid1" ? note1 : note2;
			var updated = new Bookmark
			{
				Id = toUpdate.Id,
				UID = uid,
				Archived = !toUpdate.Archived,
				Title = "new title",
				URL = "newurl"
			};

			var result = await repo.Update(updated);

			Assert.Null(result);
			AssertCollectionEquals();
		}

	}
}