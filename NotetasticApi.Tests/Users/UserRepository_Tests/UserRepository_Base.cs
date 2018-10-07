using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Tests.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.UserRepository_Tests
{

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