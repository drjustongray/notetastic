using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Tests.Common;
using NotetasticApi.Users;
using Xunit;

namespace NotetasticApi.Tests.Users.RefreshTokenRepositoryTests
{
	[Collection("Database collection")]
	public class RefreshTokenRepository_Base
	{
		private readonly IMongoCollection<RefreshToken> _collection;
		protected RefreshTokenRepository _repo;
		protected readonly RefreshToken _existing1 = new RefreshToken
		{
			UID = "uid1",
			Token = "token1",
			ExpiresAt = DateTimeOffset.Now.AddMinutes(5)
		};

		protected readonly RefreshToken _existing2 = new RefreshToken
		{
			UID = "uid2",
			Token = "token2",
			ExpiresAt = DateTimeOffset.Now.AddMinutes(2),
			Persistent = true
		};

		protected readonly RefreshToken _existing3 = new RefreshToken
		{
			UID = "uid2",
			Token = "token3",
			ExpiresAt = DateTimeOffset.Now.AddDays(10)
		};

		protected readonly HashSet<RefreshToken> _expectedTokens;

		protected HashSet<RefreshToken> _actualTokens => new HashSet<RefreshToken>(_collection.Find(new BsonDocument()).ToEnumerable());

		public RefreshTokenRepository_Base(DatabaseFixture fixture)
		{

			var database = fixture.Client.GetDatabase("IntegrationTest");
			_collection = database.GetCollection<RefreshToken>("TestAuthTokens");
			_collection.DeleteMany(new BsonDocument());

			_collection.InsertMany(new RefreshToken[] { _existing1, _existing2, _existing3 });
			Assert.NotNull(_existing1.Id);
			_expectedTokens = new HashSet<RefreshToken>();
			_expectedTokens.Add(_existing1);
			_expectedTokens.Add(_existing2);
			_expectedTokens.Add(_existing3);
			_repo = new RefreshTokenRepository(_collection);
		}

		[Fact]
		protected void AssertCollectionEquals()
		{
			Assert.Equal(_expectedTokens, _actualTokens);
		}
	}
}