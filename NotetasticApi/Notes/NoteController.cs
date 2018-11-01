
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
		private readonly IValidationService validationService;

		public NoteController(INoteRepository noteRepository, IValidationService validationService)
		{
			this.noteRepository = noteRepository;
			this.validationService = validationService;
		}

		[HttpPut]
		public async Task<ActionResult<Note>> PutNote([FromBody] Note note)
		{
			if (note == null)
			{
				return BadRequest();
			}
			note.UID = UID;
			if (!validationService.IsNoteValid(note))
			{
				return BadRequest();
			}
			if (note.Id == null)
			{
				return await noteRepository.Create(note);
			}
			else
			{
				note = await noteRepository.Update(note);
				if (note == null)
				{
					return NotFound();
				}
				return note;
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Note>> GetNote(string id)
		{
			var note = await noteRepository.FindById(id, UID);
			if (note == null)
			{
				return NotFound();
			}
			return note;
		}

		[HttpGet]
		public async Task<ActionResult<List<NoteMetaData>>> GetNoteList()
		{
			return await noteRepository.FindAllByUser(UID);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteNote(string id)
		{
			return await noteRepository.Delete(id, UID) ? Ok() : NotFound() as ActionResult;
		}
	}
}