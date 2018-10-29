using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Notes;
using NotetasticApi.Tests.Common;
using Xunit;

namespace NotetasticApi.Tests.Notes.NoteRepositoryTests
{
	[Collection("Database collection")]
	public class NoteRepository_Base
	{
		protected readonly IMongoCollection<Note> collection;
		protected NoteRepository repo;

		protected readonly Note note1 = new TextNote
		{
			UID = "uid1",
			Archived = false,
			Title = "title1",
			Text = "some text"
		};

		protected readonly Note note3 = new Location
		{
			UID = "uid1",
			Archived = false,
			Title = "home",
			Latitude = 78,
			Longitude = 45
		};

		protected readonly Note note4 = new Bookmark
		{
			UID = "uid1",
			Archived = false,
			Title = "google",
			URL = "https://www.google.com"
		};

		protected readonly Note note2 = new TextNote
		{
			UID = "uid2",
			Archived = true,
			Title = "title2",
			Text = "some more text"
		};


		protected HashSet<Note> expectedCollection => new HashSet<Note> { note1, note2, note3, note4 };

		protected HashSet<Note> actualCollection => new HashSet<Note>(collection.Find(new BsonDocument()).ToEnumerable());

		public NoteRepository_Base(DatabaseFixture fixture)
		{

			var database = fixture.Client.GetDatabase("IntegrationTest");
			collection = database.GetCollection<Note>("TestNotes");
			collection.DeleteMany(new BsonDocument());

			collection.InsertMany(new Note[] { note1, note2, note3, note4 });
			repo = new NoteRepository(collection);
		}

		protected void AssertCollectionEquals(HashSet<Note> expectedCollection = null)
		{
			expectedCollection = expectedCollection ?? this.expectedCollection;
			Assert.Equal(expectedCollection, actualCollection);
		}
	}
}