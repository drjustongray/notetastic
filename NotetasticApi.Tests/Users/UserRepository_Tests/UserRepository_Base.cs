using System;
using System.Collections.Generic;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRepository_Tests
{

	[CollectionDefinition("Database collection")]
	public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}

	public class DatabaseFixture : IDisposable
	{
		private readonly MongoDbRunner _runner;
		public DatabaseFixture()
		{
			_runner = MongoDbRunner.Start();
			Client = new MongoClient(_runner.ConnectionString);
		}

		public MongoClient Client { get; private set; }

		public void Dispose()
		{
			_runner.Dispose();
		}
	}

	[Collection("Database collection")]
	public class UserRepository_Base
	{

		private readonly IMongoCollection<User> _collection;
		protected UserRepository _repo;
		protected readonly User _existing = new User
		{
			UserName = "jane",
			PasswordHash = "don'tmatter"
		};

		protected readonly User _otherExisting = new User
		{
			UserName = "bob",
			PasswordHash = "fasdfasdf"
		};

		protected readonly HashSet<User> _expectedUsers;

		protected HashSet<User> _actualUsers => new HashSet<User>(_collection.Find(new BsonDocument()).ToEnumerable());

		public UserRepository_Base(DatabaseFixture fixture)
		{

			var database = fixture.Client.GetDatabase("IntegrationTest");
			_collection = database.GetCollection<User>("TestUsers");
			_collection.DeleteMany(new BsonDocument());

			_collection.InsertMany(new User[] { _existing, _otherExisting });
			Assert.NotNull(_existing.Id);
			_expectedUsers = new HashSet<User>();
			_expectedUsers.Add(_existing);
			_expectedUsers.Add(_otherExisting);
			_repo = new UserRepository(_collection);
		}

		[Fact]
		protected void AssertCollectionEquals()
		{
			Assert.Equal(_expectedUsers, _actualUsers);
		}
	}

}