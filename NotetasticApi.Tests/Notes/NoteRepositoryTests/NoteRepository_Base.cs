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

		protected readonly NoteBook root1 = new NoteBook
		{
			UID = "uid1",
			IsRoot = true,
			Items = new List<NoteBookItem>()
		};

		protected readonly NoteBook root2 = new NoteBook
		{
			UID = "uid2",
			IsRoot = true,
			Items = new List<NoteBookItem>()
		};

		protected readonly HashSet<Note> expectedCollection;

		protected HashSet<Note> actualCollection => new HashSet<Note>(collection.Find(new BsonDocument()).ToEnumerable());

		public NoteRepository_Base(DatabaseFixture fixture)
		{

			var database = fixture.Client.GetDatabase("IntegrationTest");
			collection = database.GetCollection<Note>("TestNotes");
			collection.DeleteMany(new BsonDocument());

			collection.InsertMany(new Note[] { root1, root2 });
			Assert.NotNull(root1.Id);
			expectedCollection = new HashSet<Note>();
			expectedCollection.Add(root1);
			expectedCollection.Add(root2);
			repo = new NoteRepository(collection);
		}

		[Fact]
		protected void AssertCollectionEquals()
		{
			Assert.Equal(expectedCollection, actualCollection);
		}
	}
}