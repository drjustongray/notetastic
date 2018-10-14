using System;
using NotetasticApi.Common;

namespace NotetasticApi.Users
{
	public class RefreshToken : Record
	{
		public string UID { get; set; }
		public string Token { get; set; }
		public DateTimeOffset? ExpiresAt { get; set; }
		public bool Persistent { get; set; }

		public bool IsValid => !(string.IsNullOrWhiteSpace(UID) || string.IsNullOrWhiteSpace(Token) || ExpiresAt == null);

		public static bool operator ==(RefreshToken a, RefreshToken b)
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

		public static bool operator !=(RefreshToken a, RefreshToken b) => !(a == b);

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var other = (RefreshToken)obj;

			return Id == other.Id && UID == other.UID && Token == other.Token && ExpiresAt == other.ExpiresAt && Persistent == other.Persistent;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, Token, ExpiresAt).GetHashCode();
		}
	}
}