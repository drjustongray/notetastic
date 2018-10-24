using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NotetasticApi.Common;

namespace NotetasticApi.Notes
{
	public interface INoteRepository
	{
		Task<Note> Create(Note note);
		Task<Note> FindById(string id, string uid);
		Task<Notebook> FindRootNotebook(string uid);
		Task<Note> Update(Note note);
		Task<bool> Delete(string id, string uid);
	}
	public class NoteRepository : INoteRepository
	{
		private readonly IMongoCollection<Note> noteCollection;
		private readonly IMongoClient client;

		public NoteRepository(IMongoCollection<Note> noteCollection, IMongoClient client)
		{
			this.noteCollection = noteCollection;
			this.client = client;
			noteCollection.Indexes.CreateOne(new CreateIndexModel<Note>(
				Builders<Note>.IndexKeys.Combine(
					Builders<Note>.IndexKeys.Ascending(_ => _.UID)
				),
				new CreateIndexOptions<Note>
				{
					Unique = true,
					PartialFilterExpression = new BsonDocument("IsRoot", true)
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

		/// <summary>
		/// deletes the matching note if it exists.
		/// If the note is a Notebook, deletes all sub-notes
		/// removes the corresponding NotebookItem from the notebook containing it
		/// does not allow the user's root notebook to be deleted
		/// returns false if there's no matching document
		/// </summary>
		/// <param name="id"></param>
		/// <param name="uid"></param>
		/// <returns></returns>
		public async Task<bool> Delete(string id, string uid)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}

			Note note;
			using (var session = await client.StartSessionAsync())
			{
				var filter = Builders<Note>.Filter.And(
					new BsonDocument("_id", id),
					new BsonDocument("UID", uid),
					Builders<Note>.Filter.Not(new BsonDocument("IsRoot", true))
				);
				session.StartTransaction();
				note = await noteCollection.FindOneAndDeleteAsync(session, filter);
				if (note == null)
				{
					await session.AbortTransactionAsync();
					return false;
				}

				// removing the note from the containing notebook's Items array
				await noteCollection.UpdateOneAsync(
					session,
					new BsonDocument("_id", note.NBID),
					Builders<Note>.Update.PullFilter<NotebookItem>("Items", new BsonDocument("_id", note.Id))
				);

				await session.CommitTransactionAsync();
			}
			if (note is Notebook)
			{
				var toExplore = new Queue<string>();
				toExplore.Enqueue(note.Id);
				var subNotebooks = new HashSet<string> { note.Id };
				while (toExplore.Count > 0)
				{
					var notebook = toExplore.Dequeue();
					// find all notebooks in this notebook
					var filter = Builders<Note>.Filter.And(
						new BsonDocument("NBID", notebook),
						new BsonDocument("_t", "Notebook")
					);
					using (var cursor = await noteCollection.FindAsync<Record>(filter))
					{
						while (await cursor.MoveNextAsync())
						{
							foreach (var doc in cursor.Current)
							{
								toExplore.Enqueue(doc.Id);
								subNotebooks.Add(doc.Id);
							}
						}
					}
				}

				// delete all documents with NBID's matching something in containedNotebookIds
				await noteCollection.DeleteManyAsync(Builders<Note>.Filter.In("NBID", subNotebooks));
			}
			return true;
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