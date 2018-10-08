using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Common;

namespace NotetasticApi.Users
{

	public interface IRefreshTokenRepository
	{
		Task<RefreshToken> Create(RefreshToken token);
		Task<RefreshToken> Find(string token);
		Task<RefreshToken> Update(RefreshToken token);
		Task Delete(string token);
		Task DeleteAll(string uid);
	}
	public class RefreshTokenRepository : IRefreshTokenRepository
	{

		private IMongoCollection<RefreshToken> _tokens;

		public RefreshTokenRepository(IMongoCollection<RefreshToken> tokens)
		{
			_tokens = tokens;
			_tokens.Indexes.CreateOne(new CreateIndexModel<RefreshToken>(
				Builders<RefreshToken>.IndexKeys.Ascending(_ => _.Token),
				new CreateIndexOptions
				{
					Unique = true
				}
			));
			_tokens.Indexes.CreateOne(new CreateIndexModel<RefreshToken>(
				Builders<RefreshToken>.IndexKeys.Ascending(_ => _.UID)
			));
		}

		public async Task<RefreshToken> Create(RefreshToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}
			if (!token.IsValid)
			{
				throw new ArgumentException($"Invalid: {token}");
			}
			if (token.Id != null)
			{
				throw new ArgumentException("Id must be null");
			}
			try
			{
				await _tokens.InsertOneAsync(token);
			}
			catch (MongoWriteException e)
			{
				if (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
				{
					throw new DocumentConflictException();
				}
				else throw e;
			}
			return token;
		}

		public async Task Delete(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}
			await _tokens.DeleteOneAsync(new BsonDocument("Token", token));
		}

		public async Task DeleteAll(string uid)
		{
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}
			await _tokens.DeleteManyAsync(new BsonDocument("UID", uid));
		}

		public async Task<RefreshToken> Find(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}
			var filter = Builders<RefreshToken>.Filter.Eq("Token", token);
			return await (await _tokens.FindAsync(filter)).FirstOrDefaultAsync();
		}

		public async Task<RefreshToken> Update(RefreshToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}
			if (!token.IsValid || token.Id == null)
			{
				throw new ArgumentException($"Invalid: {token}");
			}
			var filter = Builders<RefreshToken>.Filter.And(
				new BsonDocument("_id", token.Id),
				new BsonDocument("UID", token.UID)
			);
			try
			{
				var res = await _tokens.FindOneAndReplaceAsync(filter, token);
				if (res == null)
				{
					throw new DocumentNotFoundException();
				}
			}
			catch (MongoCommandException e)
			{
				if (e.Code == 11000)
				{
					throw new DocumentConflictException();
				}
				else throw e;
			}
			return token;
		}
	}
}