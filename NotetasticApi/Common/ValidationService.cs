using System;
using System.Globalization;
using System.Linq;

namespace NotetasticApi.Common
{

	public interface IValidationService
	{
		void AssertNonNull(object value, string varName);
		bool IsPasswordValid(string password, out string reason);
		bool IsUsernameValid(string username, out string reason);
		void ValidatePassword(string password);
		void ValidateUsername(string username);
	}

	public class ValidationService : IValidationService
	{
		public void AssertNonNull(object value, string varName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(varName);
			}
		}

		public bool IsPasswordValid(string password, out string reason)
		{
			if (password == null)
			{
				reason = "Password is missing";
				return false;
			}
			if (password.Any(Char.IsWhiteSpace))
			{
				reason = "Password must not contain white space";
				return false;
			}
			if (new StringInfo(password).LengthInTextElements < 8)
			{
				reason = "Password must be at least 8 characters long";
				return false;
			}
			reason = "";
			return true;
		}

		public bool IsUsernameValid(string username, out string reason)
		{
			if (username == null)
			{
				reason = "Username is missing";
				return false;
			}
			if (username.Any(Char.IsWhiteSpace))
			{
				reason = "Username must not contain white space";
				return false;
			}
			if (new StringInfo(username).LengthInTextElements < 3)
			{
				reason = "Username must be at least 3 characters long";
				return false;
			}
			reason = "";
			return true;
		}

		public void ValidatePassword(string password)
		{
			string reason;
			if (!IsPasswordValid(password, out reason))
			{
				throw new ArgumentException(reason);
			}
		}

		public void ValidateUsername(string username)
		{
			string reason;
			if (!IsUsernameValid(username, out reason))
			{
				throw new ArgumentException(reason);
			}
		}
	}
}