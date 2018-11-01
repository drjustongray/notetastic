using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotetasticApi.Common;
using NotetasticApi.Notes;

namespace NotetasticApi.Tests.Notes.NoteControllerTests
{
	public class NotesController_Base
	{
		protected readonly Mock<INoteRepository> noteRepository;
		protected Mock<IValidationService> validationService;
		protected readonly NotesController noteController;

		public NotesController_Base()
		{
			noteRepository = new Mock<INoteRepository>();
			validationService = new Mock<IValidationService>(MockBehavior.Strict);
			noteController = new NotesController(noteRepository.Object, validationService.Object);
		}

		protected void SetupUser(string uid)
		{
			var controllerContext = new ControllerContext();
			var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
			httpContext.SetupGet(x => x.User).Returns(
				new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(NotetasticApi.Users.ClaimTypes.UID, uid) }))
			);

			controllerContext.HttpContext = httpContext.Object;
			noteController.ControllerContext = controllerContext;
		}
	}
}