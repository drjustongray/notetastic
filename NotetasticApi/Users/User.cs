using System;
using NotetasticApi.Common;

namespace NotetasticApi.Users
{
	public class User : Record
	{
		public string UserName { get; set; }
		public string PasswordHash { get; set; }

		public bool IsValid => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(PasswordHash);

		public static bool operator ==(User a, User b)
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

		public static bool operator !=(User a, User b) => !(a == b);

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var other = (User)obj;

			return Id == other.Id && UserName == other.UserName && PasswordHash == other.PasswordHash;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UserName, PasswordHash).GetHashCode();
		}
	}
}