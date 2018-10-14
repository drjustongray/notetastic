using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NotetasticApi.Common;

namespace NotetasticApi.Users
{

	[Route("user"), Authorize]
	public class UserController : ControllerBase
	{
		public const string REFRESH_TOKEN = "REFRESH_TOKEN";
		private readonly IUserService userService;
		private readonly IValidationService validationService;

		public UserController(IUserService userService, IValidationService validationService)
		{
			this.userService = userService;
			this.validationService = validationService;
		}

		/// <summary>
		/// Expecting request with username and password values, with an optional value rememberMe
		/// purpose is to authenticate an existing user
		/// If authentication succeeds, an AuthenticationResponse is sent back with an access token, and a refresh token cookie is added
		/// if rememberMe is true, the cookie is set to be permanent
		/// </summary>
		/// <param name="auth"></param>
		/// <returns></returns>
		[HttpPost, AllowAnonymous]
		public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] AuthenticationRequest auth)
		{
			if (auth?.username == null || auth.password == null)
			{
				var modelState = new ModelStateDictionary();
				if (auth?.username == null)
				{
					modelState.AddModelError("username", "username must be present");
				}
				if (auth?.password == null)
				{
					modelState.AddModelError("password", "password must be present");
				}
				return BadRequest(modelState);
			}
			var user = await userService.Authenticate(auth.username, auth.password);
			if (user == null)
			{
				return Unauthorized();
			}

			var shouldPersist = auth.rememberMe ?? false;
			var tokens = await userService.CreateAuthTokens(user, shouldPersist);
			var cookieOptions = new CookieOptions { Secure = true, HttpOnly = true };
			if (shouldPersist)
			{
				cookieOptions.MaxAge = TimeSpan.FromDays(30);
			}
			Response.Cookies.Append(REFRESH_TOKEN, tokens.RefreshToken, cookieOptions);
			return new AuthenticationResponse
			{
				uid = user.Id,
				username = user.UserName,
				token = tokens.AccessToken
			};
		}

		/// <summary>
		/// Expecting request with username and password values, with an optional value rememberMe
		/// purpose is to create a new user account
		/// If account creation succeeds, an AuthenticationResponse is sent back with an access token, and a refresh token cookie is added
		/// if rememberMe is true, the cookie is set to be permanent
		/// </summary>
		/// <param name="auth"></param>
		/// <returns></returns>
		[HttpPut, AllowAnonymous]
		public async Task<ActionResult<AuthenticationResponse>> Register([FromBody] AuthenticationRequest auth)
		{
			string usernameIssue;
			string passwordIssue;
			var usernameProblem = !validationService.IsUsernameValid(auth?.username, out usernameIssue);
			var passwordProblem = !validationService.IsPasswordValid(auth?.password, out passwordIssue);
			if (usernameProblem || passwordProblem)
			{
				var modelState = new ModelStateDictionary();
				if (usernameProblem)
				{
					modelState.AddModelError("username", usernameIssue);
				}
				if (passwordProblem)
				{
					modelState.AddModelError("password", passwordIssue);
				}
				return BadRequest(modelState);
			}
			var user = await userService.CreateAccount(auth.username, auth.password);
			if (user == null)
			{
				return Conflict();
			}
			var shouldPersist = auth.rememberMe ?? false;
			var tokens = await userService.CreateAuthTokens(user, shouldPersist);
			var cookieOptions = new CookieOptions { Secure = true, HttpOnly = true };
			if (shouldPersist)
			{
				cookieOptions.MaxAge = TimeSpan.FromDays(30);
			}
			Response.Cookies.Append(REFRESH_TOKEN, tokens.RefreshToken, cookieOptions);
			return new AuthenticationResponse
			{
				uid = user.Id,
				username = user.UserName,
				token = tokens.AccessToken
			};
		}

		/// <summary>
		/// checks for a refresh_token cookie. 
		/// If found, generates an access token and sends a full authentication response along with a fresh refresh token
		/// </summary>
		/// <param name="userRepo"></param>
		/// <returns></returns>
		[HttpGet, AllowAnonymous]
		public async Task<ActionResult<AuthenticationResponse>> GetUserAuth()
		{
			return null;
		}

		[HttpPut("username")]
		public async Task<ActionResult<AuthenticationResponse>> ChangeUsername([FromBody] AuthenticationRequest auth)
		{
			return null;
		}

		[HttpPut("password")]
		public async Task<ActionResult<AuthenticationResponse>> ChangePassword([FromBody] AuthenticationRequest auth)
		{
			return null;
		}

		/// <summary>
		/// deletes the refresh token present in the cookie (if there is one), and removes it from the response?
		/// </summary>
		/// <returns></returns>
		[HttpGet("logout"), AllowAnonymous]
		public async Task<ActionResult> Logout()
		{
			return null;
		}

		[HttpGet("logoutall")]
		public async Task<ActionResult> LogoutAll()
		{
			return null;
		}

		private string UID
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