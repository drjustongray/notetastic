using System;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	public class NoteRepository_FindById : NoteRepository_Base
	{
		public NoteRepository_FindById(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Theory]
		[InlineData(null, "uid")]
		[InlineData("id", null)]
		public async void ThrowsIfArgMissing(string id, string uid)
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => repo.FindById(id, uid)
			);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void ReturnsNoteIfMatch(string uid)
		{
			var note = uid == note1.UID ? note1 : note2;
			Assert.Equal(note, await repo.FindById(note.Id, uid));
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void ReturnsNullIfRoot(string uid)
		{
			var root = uid == root1.UID ? root1 : root2;
			Assert.Null(await repo.FindById(root.Id, uid));
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void ReturnsNullIfUIDMisMatch(string uid)
		{
			var id = uid == note1.UID ? note2.Id : note1.Id;
			Assert.Null(await repo.FindById(id, uid));
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("id1", "uid")]
		[InlineData("id2", "uid")]
		public async void ReturnsNullIfNoteNotFound(string id, string uid)
		{
			Assert.Null(await repo.FindById(id, uid));
			AssertCollectionEquals();
		}
	}
}