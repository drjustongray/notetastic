using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Tests.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.AuthTokenRepositoryTests
{
	[Collection("Database collection")]
	public class AuthTokenRepository_Base
	{
		private readonly IMongoCollection<AuthToken> _collection;
		protected AuthTokenRepository _repo;
		protected readonly AuthToken _existing1 = new AuthToken
		{
			UID = "uid1",
			Token = "token1",
			ExpiresAt = DateTimeOffset.Now.AddMinutes(5)
		};

		protected readonly AuthToken _existing2 = new AuthToken
		{
			UID = "uid2",
			Token = "token2",
			ExpiresAt = DateTimeOffset.Now.AddMinutes(2)
		};

		protected readonly AuthToken _existing3 = new AuthToken
		{
			UID = "uid2",
			Token = "token3",
			ExpiresAt = DateTimeOffset.Now.AddDays(10)
		};

		protected readonly HashSet<AuthToken> _expectedTokens;

		protected HashSet<AuthToken> _actualUsers => new HashSet<AuthToken>(_collection.Find(new BsonDocument()).ToEnumerable());

		public AuthTokenRepository_Base(DatabaseFixture fixture)
		{

			var database = fixture.Client.GetDatabase("IntegrationTest");
			_collection = database.GetCollection<AuthToken>("TestAuthTokens");
			_collection.DeleteMany(new BsonDocument());

			_collection.InsertMany(new AuthToken[] { _existing1, _existing2, _existing3 });
			Assert.NotNull(_existing1.Id);
			_expectedTokens = new HashSet<AuthToken>();
			_expectedTokens.Add(_existing1);
			_expectedTokens.Add(_existing2);
			_expectedTokens.Add(_existing3);
			_repo = new AuthTokenRepository(_collection);
		}

		[Fact]
		protected void AssertCollectionEquals()
		{
			Assert.Equal(_expectedTokens, _actualUsers);
		}
	}
}