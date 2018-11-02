using System.Threading.Tasks;
using NotetasticApi.Common;

namespace NotetasticApi.Users
{

	public interface IUserService
	{
		Task<User> Authenticate(string username, string password);
		Task<TokenPair> CreateAuthTokens(User user, bool isPersistent);
		Task<TokenPair> CreateAuthTokens(string refreshToken);
		Task<User> CreateAccount(string username, string password);
		Task<User> ChangePassword(string uid, string password, string newPassword);
		Task<User> ChangeUsername(string uid, string username, string password);
		Task RevokeRefreshToken(string token);
		Task RevokeAllRefreshTokens(string uid);
	}

	public class UserService : IUserService
	{
		private readonly IUserRepository userRepo;
		private readonly IRefreshTokenRepository refreshTokenRepo;
		private readonly IPasswordService passwordService;
		private readonly ITokenService tokenService;
		private readonly IValidationService validationService;
		private readonly ITimeService timeService;

		public UserService(
			IUserRepository userRepo,
			IRefreshTokenRepository refreshTokenRepo,
			IPasswordService passwordService,
			ITokenService tokenService,
			IValidationService validationService,
			ITimeService timeService
		)
		{
			this.userRepo = userRepo;
			this.refreshTokenRepo = refreshTokenRepo;
			this.passwordService = passwordService;
			this.tokenService = tokenService;
			this.validationService = validationService;
			this.timeService = timeService;
		}

		public async Task<User> Authenticate(string username, string password)
		{
			validationService.AssertNonNull(username, nameof(username));
			validationService.AssertNonNull(password, nameof(password));
			var user = await userRepo.FindByUserName(username);
			if (user != null && passwordService.Verify(password, user.PasswordHash))
			{
				return user;
			}
			return null;
		}

		public async Task<User> ChangePassword(string uid, string password, string newPassword)
		{
			validationService.AssertNonNull(uid, nameof(uid));
			validationService.AssertNonNull(password, nameof(password));
			validationService.ValidatePassword(newPassword);
			var user = await userRepo.FindById(uid);
			if (user == null)
			{
				throw new DocumentNotFoundException();
			}
			if (passwordService.Verify(password, user.PasswordHash))
			{
				var hash = passwordService.Hash(newPassword);
				return await userRepo.UpdatePasswordHash(uid, hash);
			}
			return null;
		}

		public async Task<User> ChangeUsername(string uid, string username, string password)
		{
			validationService.AssertNonNull(uid, nameof(uid));
			validationService.AssertNonNull(password, nameof(password));
			validationService.ValidateUsername(username);
			var user = await userRepo.FindById(uid);
			if (user == null)
			{
				throw new DocumentNotFoundException();
			}
			if (passwordService.Verify(password, user.PasswordHash))
			{
				return await userRepo.UpdateUserName(uid, username);
			}
			return null;
		}

		public async Task<User> CreateAccount(string username, string password)
		{
			validationService.ValidateUsername(username);
			validationService.ValidatePassword(password);
			try
			{
				return await userRepo.Create(new User
				{
					UserName = username,
					PasswordHash = passwordService.Hash(password)
				});
			}
			catch (DocumentConflictException)
			{
				return null;
			}
		}

		/// <summary>
		/// Creates an access token and refresh token for the user
		/// saves refresh token to database
		/// refresh token will be set to expire after 30 days
		/// </summary>
		/// <param name="user"></param>
		/// <returns>New Access and Refresh Tokens (to send to client)</returns>
		public async Task<TokenPair> CreateAuthTokens(User user, bool isPersistent)
		{
			validationService.AssertNonNull(user, nameof(user));
			validationService.AssertNonNull(user.Id, nameof(user.Id));
			var refreshToken = new RefreshToken { UID = user.Id, Persistent = isPersistent };
			try
			{
				await CreateToken(refreshToken);
			}
			catch (DocumentConflictException)
			{
				await CreateToken(refreshToken);
			}

			return new TokenPair
			{
				RefreshToken = refreshToken.Token,
				AccessToken = tokenService.CreateAccessToken(user.Id),
				User = user,
				Persistent = isPersistent
			};
		}

		/// <summary>
		/// Creates an access token and refresh token for the user associated with the provided token
		/// saves the new refresh token to database, overwriting the old token
		/// refresh token will be set to expire after 30 days
		/// </summary>
		/// <param name="refreshToken"></param>
		/// <returns>New Access and Refresh Tokens (to send to client), or null if the token is invalid or expired</returns>
		public async Task<TokenPair> CreateAuthTokens(string refreshToken)
		{
			validationService.AssertNonNull(refreshToken, nameof(refreshToken));
			var tokenDoc = await refreshTokenRepo.Find(refreshToken);
			if (tokenDoc == null || tokenDoc.ExpiresAt < timeService.GetCurrentTime())
			{
				return null;
			}
			try
			{
				await UpdateToken(tokenDoc);
			}
			catch (DocumentConflictException)
			{
				await UpdateToken(tokenDoc);
			}

			return new TokenPair
			{
				RefreshToken = tokenDoc.Token,
				AccessToken = tokenService.CreateAccessToken(tokenDoc.UID),
				User = await userRepo.FindById(tokenDoc.UID),
				Persistent = tokenDoc.Persistent
			};
		}

		public Task RevokeAllRefreshTokens(string uid)
		{
			validationService.AssertNonNull(uid, nameof(uid));
			return refreshTokenRepo.DeleteAll(uid);
		}

		public Task RevokeRefreshToken(string token)
		{
			validationService.AssertNonNull(token, nameof(token));
			return refreshTokenRepo.Delete(token);
		}

		private async Task CreateToken(RefreshToken refreshToken)
		{
			refreshToken.Token = tokenService.CreateRefreshToken();
			refreshToken.ExpiresAt = timeService.GetCurrentTime().AddDays(30);
			await refreshTokenRepo.Create(refreshToken);
		}

		private async Task UpdateToken(RefreshToken tokenDoc)
		{
			tokenDoc.ExpiresAt = timeService.GetCurrentTime().AddDays(30);
			tokenDoc.Token = tokenService.CreateRefreshToken();
			await refreshTokenRepo.Update(tokenDoc);
		}
	}
}