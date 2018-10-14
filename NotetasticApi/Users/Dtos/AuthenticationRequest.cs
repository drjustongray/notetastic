namespace NotetasticApi.Users
{
	public class AuthenticationRequest
	{
		public string username { get; set; }
		public string password { get; set; }
		public string newUsername { get; set; }
		public string newPassword { get; set; }
		public bool? rememberMe { get; set; }
	}
}