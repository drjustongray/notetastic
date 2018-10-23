using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NotetasticApi.Notes
{
	public interface INoteRepository
	{
		Task<Note> Create(Note note);
		Task<Note> FindById(string id, string uid);
		Task<Notebook> FindRootNotebook(string uid);
		Task<Note> Update(Note note);
		Task Delete(string id, string uid);
	}
	public class NoteRepository : INoteRepository
	{
		private readonly IMongoCollection<Note> noteCollection;

		public NoteRepository(IMongoCollection<Note> noteCollection)
		{
			this.noteCollection = noteCollection;
			noteCollection.Indexes.CreateOne(new CreateIndexModel<Note>(
				Builders<Note>.IndexKeys.Combine(
					Builders<Note>.IndexKeys.Ascending(_ => _.UID),
					new BsonDocument("IsRoot", 1)
				),
				new CreateIndexOptions
				{
					Unique = true,
					Sparse = true
				}
			));
		}

		/// <summary>
		/// adds any valid note to repository
		/// NoteBooks will be foced into an 'empty' state first
		/// if NBID missing,the value for user's root notebook is used
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public Task<Note> Create(Note note)
		{
			throw new System.NotImplementedException();
		}

		public Task Delete(string id, string uid)
		{
			throw new System.NotImplementedException();
		}

		public Task<Note> FindById(string id, string uid)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// retrieves the user's root notebook, creating it if necessary
		/// </summary>
		/// <param name="uid"></param>
		/// <returns></returns>
		public async Task<Notebook> FindRootNotebook(string uid)
		{
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}
			var filter = Builders<Note>.Filter.And(
				Builders<Note>.Filter.Eq("UID", uid),
				Builders<Note>.Filter.Eq("IsRoot", true)
			);
			var root = await (await noteCollection.FindAsync(filter)).FirstOrDefaultAsync() as Notebook;
			if (root == null)
			{
				// just in case another thread/machine is trying to create the root at the same time
				try
				{
					root = new Notebook { UID = uid, IsRoot = true };
					await noteCollection.InsertOneAsync(root);
				}
				catch (MongoWriteException e)
				{
					if (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
					{
						return await (await noteCollection.FindAsync(filter)).FirstOrDefaultAsync() as Notebook;
					}
					throw e;
				}
			}
			return root;
		}

		/// <summary>
		/// replaces the note in the repo with a matchin id and uid
		/// exception: notebooks will just have certain properties updated, as the Items property is managed by the repo
		/// if NBID missing,the value for user's root notebook is used
		/// the root notebook cannot be modified
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public Task<Note> Update(Note note)
		{
			throw new System.NotImplementedException();
		}
	}
}