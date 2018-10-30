using System;
using System.Collections.Generic;
using NotetasticApi.Notes;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	public class NoteRepository_FindAllByUser : NoteRepository_Base
	{
		public NoteRepository_FindAllByUser(DatabaseFixture fixture) : base(fixture)
		{
		}

		[Fact]
		public async void ThrowsIfArgMissing()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(
				() => repo.FindAllByUser(null)
			);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsEmptyListIfNone()
		{
			var results = await repo.FindAllByUser("someid");
			Assert.Empty(results);
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsCorrectlyUID1()
		{
			var results = await repo.FindAllByUser("uid1");
			Assert.Equal(new HashSet<NoteMetaData> {
				new NoteMetaData { Id = note1.Id, Title = note1.Title, Type = note1.GetType().Name },
				new NoteMetaData { Id = note3.Id, Title = note3.Title, Type = note3.GetType().Name },
				new NoteMetaData { Id = note4.Id, Title = note4.Title, Type = note4.GetType().Name }
			}, new HashSet<NoteMetaData>(results));
			AssertCollectionEquals();
		}

		[Fact]
		public async void ReturnsCorrectlyUID2()
		{
			var results = await repo.FindAllByUser("uid2");
			Assert.Equal(new HashSet<NoteMetaData> {
				new NoteMetaData { Id = note2.Id, Title = note2.Title, Type = note2.GetType().Name }
			}, new HashSet<NoteMetaData>(results));
			AssertCollectionEquals();
		}
	}
}