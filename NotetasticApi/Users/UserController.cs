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
	public class UserController : BaseController
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
				return BadRequest(
					usernameProblem: auth?.username == null ? "username must be present" : null,
					passwordProblem: auth?.password == null ? "password must be present" : null
				);
			}
			var user = await userService.Authenticate(auth.username, auth.password);
			if (user == null)
			{
				return Unauthorized();
			}

			var shouldPersist = auth.rememberMe ?? false;
			var tokens = await userService.CreateAuthTokens(user, shouldPersist);
			SetRefreshToken(tokens.RefreshToken, shouldPersist);
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
				return BadRequest(
					usernameProblem: usernameProblem ? usernameIssue : null,
					passwordProblem: passwordProblem ? passwordIssue : null
				);
			}
			var user = await userService.CreateAccount(auth.username, auth.password);
			if (user == null)
			{
				return Conflict();
			}
			var shouldPersist = auth.rememberMe ?? false;
			var tokens = await userService.CreateAuthTokens(user, shouldPersist);
			SetRefreshToken(tokens.RefreshToken, shouldPersist);
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
			var refreshToken = GetRefreshToken();
			if (refreshToken == null)
			{
				return base.BadRequest();
			}
			var tokenPair = await userService.CreateAuthTokens(refreshToken);
			if (tokenPair == null)
			{
				return Unauthorized();
			}
			SetRefreshToken(tokenPair.RefreshToken, tokenPair.Persistent);
			return new AuthenticationResponse
			{
				uid = tokenPair.User.Id,
				username = tokenPair.User.UserName,
				token = tokenPair.AccessToken
			};
		}

		/// <summary>
		/// If newUsername or newPassword (not both) and a password are present, then an attempt is made to update that value on the user
		/// </summary>
		/// <param name="auth"></param>
		/// <returns></returns>
		[HttpPatch]
		public async Task<ActionResult<AuthenticationResponse>> UpdateUser([FromBody] AuthenticationRequest auth)
		{
			if (auth == null)
			{
				return BadRequest("Incorrect Format");
			}
			var currentPassword = auth.password;
			if (currentPassword == null)
			{
				return BadRequest("Current Password Must Be Provided");
			}
			var newPassword = auth.newPassword;
			var newUsername = auth.newUsername;
			if ((newPassword == null) == (newUsername == null))
			{
				return BadRequest("Must Provide Exactly One Of: New Password, New Username");
			}
			User user;
			if (newUsername != null)
			{
				string message;
				var passed = validationService.IsUsernameValid(newUsername, out message);
				if (!passed)
				{
					return BadRequest(usernameProblem: message);
				}
				user = await userService.ChangeUsername(UID, newUsername, currentPassword);
			}
			else
			{
				string message;
				var passed = validationService.IsPasswordValid(newPassword, out message);
				if (!passed)
				{
					return BadRequest(passwordProblem: message);
				}
				user = await userService.ChangePassword(UID, currentPassword, newPassword);
			}

			if (user == null)
			{
				return Unauthorized();
			}
			return new AuthenticationResponse { uid = UID, username = newUsername ?? user.UserName };
		}

		/// <summary>
		/// deletes the refresh token present in the cookie (if there is one), and removes it from the response?
		/// </summary>
		/// <returns></returns>
		[HttpGet("logout"), AllowAnonymous]
		public async Task<ActionResult> Logout()
		{
			var refreshToken = GetRefreshToken();
			if (refreshToken == null)
			{
				return base.BadRequest();
			}
			await userService.RevokeRefreshToken(refreshToken);
			RemoveRefreshToken();
			return NoContent();
		}

		[HttpGet("logoutall")]
		public async Task<ActionResult> LogoutAll()
		{
			await userService.RevokeAllRefreshTokens(UID);
			RemoveRefreshToken();
			return NoContent();
		}

		private BadRequestObjectResult BadRequest(string message = null, string usernameProblem = null, string passwordProblem = null)
		{
			var dict = new ModelStateDictionary();
			if (message != null)
			{
				dict.AddModelError("message", message);
			}
			if (usernameProblem != null)
			{
				dict.AddModelError("username", usernameProblem);
			}
			if (passwordProblem != null)
			{
				dict.AddModelError("password", passwordProblem);
			}
			return BadRequest(dict);
		}
		private string GetRefreshToken()
		{
			if (Request.Cookies.ContainsKey(REFRESH_TOKEN))
			{
				var token = Request.Cookies[REFRESH_TOKEN];
				if (!string.IsNullOrEmpty(token))
				{
					return token;
				}
			}
			return null;
		}

		private void SetRefreshToken(string token, bool shouldPersist)
		{
			var cookieOptions = new CookieOptions { Secure = true, HttpOnly = true };
			if (shouldPersist)
			{
				cookieOptions.MaxAge = TimeSpan.FromDays(30);
			}
			Response.Cookies.Append(REFRESH_TOKEN, token, cookieOptions);
		}

		private void RemoveRefreshToken()
		{
			Response.Cookies.Delete(REFRESH_TOKEN);
		}

	}
}