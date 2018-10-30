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
		Task<List<NoteMetaData>> FindAllByUser(string uid);
		Task<Note> Update(Note note);
		Task<bool> Delete(string id, string uid);
	}
	public class NoteRepository : INoteRepository
	{
		private static FilterDefinition<Note> HasType(Type type) => new BsonDocument("_t", type.Name);
		private static FilterDefinition<Note> OwnedBy(string uid) => new BsonDocument("UID", uid);
		private static FilterDefinition<Note> HasId(string id) => new BsonDocument("_id", id);

		private readonly IMongoCollection<Note> noteCollection;

		public NoteRepository(IMongoCollection<Note> noteCollection)
		{
			this.noteCollection = noteCollection;
		}

		/// <summary>
		/// adds any valid note to repository, throws if argument null or invalid
		/// NoteBooks will be foced into an 'empty' state first
		/// all notes will have archived set to false
		/// if NBID missing,the value for user's root notebook is used
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public async Task<Note> Create(Note note)
		{
			if (note == null)
			{
				throw new ArgumentNullException(nameof(note));
			}
			note.Archived = false;
			if (!note.IsValid || note.Id != null)
			{
				throw new ArgumentException($"Note invalid: {note}");
			}
			await noteCollection.InsertOneAsync(note);
			return note;

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

			var filter = Builders<Note>.Filter.And(HasId(id), OwnedBy(uid));
			var note = await noteCollection.FindOneAndDeleteAsync(filter);

			return note != null;
		}

		public async Task<List<NoteMetaData>> FindAllByUser(string uid)
		{
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}
			return await (await noteCollection.FindAsync<NoteMetaData>(OwnedBy(uid))).ToListAsync();
		}

		/// <summary>
		/// retrieves the maching note from the db, if it exists.
		/// the root notebook is excluded
		/// </summary>
		/// <param name="id"></param>
		/// <param name="uid"></param>
		/// <returns></returns>
		public async Task<Note> FindById(string id, string uid)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}
			if (uid == null)
			{
				throw new ArgumentNullException(nameof(uid));
			}
			var filter = Builders<Note>.Filter.And(HasId(id), OwnedBy(uid));
			return await noteCollection.FindOneAsync(filter);
		}

		/// <summary>
		/// replaces the note in the repo with a matchin id and uid
		/// exception: notebooks will just have certain properties updated, as the Items property is managed by the repo
		/// if NBID missing,the value for user's root notebook is used
		/// if the NBID is modified: ownership and existence of notebooks is verified, Items arrays on both are updated
		/// cycles are prevented
		/// the type of the note cannot change
		/// the root notebook cannot be modified
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public async Task<Note> Update(Note note)
		{
			if (note == null)
			{
				throw new ArgumentNullException(nameof(note));
			}
			if (!note.IsValid || note.Id == null)
			{
				throw new ArgumentException($"Note invalid: {note}");
			}

			var filter = Builders<Note>.Filter.And(HasId(note.Id), OwnedBy(note.UID), HasType(note.GetType()));

			var old = await noteCollection.FindOneAndReplaceAsync(filter, note);


			return old != null ? note : old;
		}
	}
}
