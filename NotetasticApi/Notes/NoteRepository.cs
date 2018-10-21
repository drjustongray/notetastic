using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace NotetasticApi.Notes
{
	public interface INoteRepository
	{
		Task<Note> Create(Note note);
		Task<Note> FindById(string id, string uid);
		Task<NoteBook> FindRootNotebook(string uid);
		Task<Note> Update(Note note);
		Task Delete(string id, string uid);
	}
	public class NoteRepository : INoteRepository
	{
		private readonly IMongoCollection<Note> noteCollection;

		public NoteRepository(IMongoCollection<Note> noteCollection)
		{
			this.noteCollection = noteCollection;
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
		public Task<NoteBook> FindRootNotebook(string uid)
		{
			throw new System.NotImplementedException();
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