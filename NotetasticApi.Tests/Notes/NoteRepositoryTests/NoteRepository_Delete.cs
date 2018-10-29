using System;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	public class NoteRepository_Delete : NoteRepository_Base
	{
		public NoteRepository_Delete(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Theory]
		[InlineData(null, "uid")]
		[InlineData("id", null)]
		public async void ThrowsIfArgMissing(string id, string uid)
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => repo.Delete(id, uid)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("id1", "sdf")]
		[InlineData("id2", "uid1")]
		[InlineData("id3", "uid2")]
		public async void ReturnsFalseDoesNothingIfNoMatchingNote(string id, string uid)
		{
			Assert.False(await repo.Delete(id, uid));
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void ReturnsFalseDoesNothingIfUIDMisMatch(string uid)
		{
			var id = uid == note1.UID ? note2.Id : note1.Id;
			Assert.False(await repo.Delete(id, uid));
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void ReturnTrueDeletesSingleNoteIfMatch(string uid)
		{
			var note = uid == note1.UID ? note1 : note2;
			Assert.True(await repo.Delete(note.Id, uid));
			var expected = expectedCollection;
			expected.Remove(note);
			AssertCollectionEquals(expected);
		}

	}
}