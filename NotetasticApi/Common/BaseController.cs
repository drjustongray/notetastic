using Microsoft.AspNetCore.Mvc;
using NotetasticApi.Users;

namespace NotetasticApi.Common
{
	[Route("api/[controller]")]
	public class BaseController : ControllerBase
	{
		protected string UID
		{
			get
			{
				foreach (var claim in User.Claims)
				{
					if (claim.Type == ClaimTypes.UID)
					{
						return claim.Value;
					}
				}
				return null;
			}
		}
	}
}