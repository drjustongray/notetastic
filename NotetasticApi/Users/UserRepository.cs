using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Common;
using System;
using System.Threading.Tasks;

namespace NotetasticApi.Users
{

	public interface IUserRepository
	{
		Task<User> FindById(string id);
		Task<User> FindByUserName(string userName);
		Task<User> Create(User user);
		Task<User> UpdateUserName(string id, string userName);
		Task<User> UpdatePasswordHash(string id, string hash);
	}
	public class UserRepository : IUserRepository
	{
		private IMongoCollection<User> _users;

		public UserRepository(IMongoCollection<User> users)
		{
			_users = users;
			_users.Indexes.CreateOne(new CreateIndexModel<User>(
				Builders<User>.IndexKeys.Ascending(_ => _.UserName),
				new CreateIndexOptions
				{
					Unique = true
				}
			));
		}
		public async Task<User> Create(User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			if (!user.IsValid)
			{
				throw new ArgumentException($"Invalid: {user}");
			}
			if (user.Id != null)
			{
				throw new ArgumentException("Id must be null");
			}
			try
			{
				await _users.InsertOneAsync(user);
			}
			catch (MongoWriteException e)
			{
				if (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
				{
					throw new DocumentConflictException();
				}
				else throw e;
			}
			return user;
		}

		public async Task<User> FindById(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}
			var filter = new BsonDocument("_id", id);
			return await _users.FindOneAsync(filter);
		}

		public async Task<User> FindByUserName(string userName)
		{
			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}
			var filter = new BsonDocument("UserName", userName);
			return await _users.FindOneAsync(filter);
		}

		public async Task<User> UpdatePasswordHash(string id, string hash)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}
			if (hash == null)
			{
				throw new ArgumentNullException(nameof(hash));
			}
			var filter = Builders<User>.Filter.Eq("_id", id);
			var update = Builders<User>.Update.Set("PasswordHash", hash);

			var user = await _users.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<User, User>
			{
				ReturnDocument = ReturnDocument.After
			});
			if (user == null)
			{
				throw new DocumentNotFoundException($"id: {id}");
			}
			return user;

		}

		public async Task<User> UpdateUserName(string id, string userName)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}
			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}
			var filter = Builders<User>.Filter.Eq("_id", id);
			var update = Builders<User>.Update.Set("UserName", userName);
			try
			{
				var user = await _users.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<User, User>
				{
					ReturnDocument = ReturnDocument.After
				});
				if (user == null)
				{
					throw new DocumentNotFoundException($"id: {id}");
				}
				return user;
			}
			catch (MongoCommandException e)
			{
				if (e.Code == 11000)
				{
					throw new DocumentConflictException();
				}
				else throw e;
			}
		}
	}
}