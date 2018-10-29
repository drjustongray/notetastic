using System;

namespace NotetasticApi.Notes
{
	public class Bookmark : Note
	{
		public string URL { get; set; }

		public override bool IsValid => base.IsValid && URL != null;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (Bookmark)obj;
			return base.Equals(other) && URL == other.URL;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, Archived, Title, URL).GetHashCode();
		}

		public static bool operator ==(Bookmark a, Bookmark b)
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

		public static bool operator !=(Bookmark a, Bookmark b) => !(a == b);
	}
}