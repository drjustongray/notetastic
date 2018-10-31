
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotetasticApi.Common;

namespace NotetasticApi.Notes
{
	[Route("notes"), Authorize]
	public class NoteController : BaseController
	{
		private readonly INoteRepository noteRepository;

		public NoteController(INoteRepository noteRepository)
		{
			this.noteRepository = noteRepository;
		}

		public async Task<ActionResult<Note>> PutNote(Note note)
		{
			return null;
		}

		public async Task<ActionResult<Note>> GetNote(string id)
		{
			return null;
		}

		public async Task<ActionResult<List<NoteMetaData>>> GetNoteList()
		{
			return null;
		}

		public async Task<ActionResult> DeleteNote(string id)
		{
			return null;
		}
	}
}