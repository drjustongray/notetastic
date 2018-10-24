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

		protected readonly Notebook root1 = new Notebook
		{
			UID = "uid1",
			IsRoot = true,
			Items = new List<NotebookItem>() {
				new NotebookItem { Title = "title1", Type = "TextNote" } ,
				new NotebookItem { Title = "title3", Type = "Notebook" }
			}
		};

		// in notebook root1
		protected readonly Note note1 = new TextNote
		{
			UID = "uid1",
			Archived = false,
			Title = "title1",
			Text = "some text"
		};

		// in notebook root1
		protected readonly Notebook notebook1 = new Notebook
		{
			UID = "uid1",
			Archived = false,
			Title = "title3",
			Items = new List<NotebookItem>() {
				new NotebookItem { Title = "home", Type = "Location" } ,
				new NotebookItem { Title = "title4", Type = "Notebook" }
			}
		};

		// in notebook notebook1
		protected readonly Note note3 = new Location
		{
			UID = "uid1",
			Archived = false,
			Title = "home",
			Latitude = 78,
			Longitude = 45
		};

		// in notebook notebook1
		protected readonly Notebook notebook2 = new Notebook
		{
			UID = "uid1",
			Archived = false,
			Title = "title4",
			Items = new List<NotebookItem>() {
				new NotebookItem { Title = "home", Type = "Location" }
			}
		};

		// in notebook notebook2
		protected readonly Note note4 = new Bookmark
		{
			UID = "uid1",
			Archived = false,
			Title = "google",
			URL = "https://www.google.com"
		};

		protected readonly Notebook root2 = new Notebook
		{
			UID = "uid2",
			IsRoot = true,
			Items = new List<NotebookItem>() { new NotebookItem { Title = "title2", Type = "TextNote" } }
		};

		// in notebook root2
		protected readonly Note note2 = new TextNote
		{
			UID = "uid2",
			Archived = true,
			Title = "title2",
			Text = "some more text"
		};

		protected HashSet<Note> expectedCollection => new HashSet<Note>
			 { root1, root2, note1, note2, note3, note4, notebook1, notebook2 };

		protected HashSet<Note> actualCollection => new HashSet<Note>(collection.Find(new BsonDocument()).ToEnumerable());

		public NoteRepository_Base(DatabaseFixture fixture)
		{

			var database = fixture.Client.GetDatabase("IntegrationTest");
			collection = database.GetCollection<Note>("TestNotes");
			collection.DeleteMany(new BsonDocument());

			collection.InsertMany(new Note[] { root1, root2 });
			note1.NBID = root1.Id;
			notebook1.NBID = root1.Id;
			note2.NBID = root2.Id;
			collection.InsertMany(new Note[] { note1, note2, notebook1 });
			root1.Items[0].Id = note1.Id;
			root1.Items[1].Id = notebook1.Id;
			root2.Items[0].Id = note2.Id;
			collection.ReplaceOne(new BsonDocument("_id", root1.Id), root1);
			collection.ReplaceOne(new BsonDocument("_id", root2.Id), root2);
			note3.NBID = notebook1.Id;
			notebook2.NBID = notebook1.Id;
			collection.InsertMany(new Note[] { note3, notebook2 });
			notebook1.Items[0].Id = note3.Id;
			notebook1.Items[1].Id = notebook2.Id;
			collection.ReplaceOne(new BsonDocument("_id", notebook1.Id), notebook1);
			note4.NBID = notebook2.Id;
			collection.InsertOne(note4);
			notebook2.Items[0].Id = note4.Id;
			collection.ReplaceOne(new BsonDocument("_id", notebook2.Id), notebook2);
			repo = new NoteRepository(collection, fixture.Client);
		}

		protected void AssertCollectionEquals(HashSet<Note> expectedCollection = null)
		{
			expectedCollection = expectedCollection ?? this.expectedCollection;
			Assert.Equal(expectedCollection, actualCollection);
		}
	}
}