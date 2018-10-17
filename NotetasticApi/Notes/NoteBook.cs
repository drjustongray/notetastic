using System;

namespace NotetasticApi.Notes
{
	public class NoteBook : Note
	{
		public int Count { get; set; }

		public override bool IsValid => UID != null && Title != null && Count >= 0;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (NoteBook)obj;
			return base.Equals(other) && Count == other.Count;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, NBID, Archived, Title, Count).GetHashCode();
		}

		public static bool operator ==(NoteBook a, NoteBook b)
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

		public static bool operator !=(NoteBook a, NoteBook b) => !(a == b);
	}
}