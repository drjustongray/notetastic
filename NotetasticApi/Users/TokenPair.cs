using System;

namespace NotetasticApi.Users
{
	public class TokenPair
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (TokenPair)obj;

			return AccessToken == other.AccessToken && RefreshToken == other.RefreshToken;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return Tuple.Create(AccessToken, RefreshToken).GetHashCode();
		}

		public static bool operator ==(TokenPair a, TokenPair b)
		{
			if (Object.ReferenceEquals(a, null))
			{
				if (Object.ReferenceEquals(b, null))
				{
					return true;
				}
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator !=(TokenPair a, TokenPair b) => !(a == b);
	}
}