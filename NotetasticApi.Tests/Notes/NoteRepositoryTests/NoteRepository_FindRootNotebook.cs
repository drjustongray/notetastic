using System;
using System.Linq;
using NotetasticApi.Notes;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	public class NoteRepository_FindRootNotebook : NoteRepository_Base
	{
		public NoteRepository_FindRootNotebook(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Theory]
		[InlineData("uid1")]
		[InlineData("uid2")]
		public async void ReturnsRootNotebookIfItExists(string uid)
		{
			var notebook = await repo.FindRootNotebook(uid);
			Assert.Equal(uid == root1.UID ? root1 : root2, notebook);
			AssertCollectionEquals();
		}

		[Theory]
		[InlineData("uid3")]
		[InlineData("uid4")]
		public async void CreatesAndReturnsRootNotebookIfItDoesNotExist(string uid)
		{
			await repo.FindRootNotebook(uid);
			var note = actualCollection.Single(_ => _ is Notebook && ((Notebook)_).IsRoot && _.UID == uid);
			var notebook = Assert.IsAssignableFrom<Notebook>(note);
			Assert.True(notebook.IsRoot);
			Assert.Null(notebook.NBID);
			Assert.Null(notebook.Archived);
			Assert.Null(notebook.Title);
			expectedCollection.Add(notebook);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ThrowsIfArgumentNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => repo.FindRootNotebook(null)
			);
		}
	}
}