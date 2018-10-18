using System;

namespace NotetasticApi.Notes
{
	public class TextNote : Note
	{
		public string Text { get; set; }

		public override bool IsValid => base.IsValid && Text != null;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (TextNote)obj;
			return base.Equals(other) && Text == other.Text;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, NBID, Archived, Title, Text).GetHashCode();
		}

		public static bool operator ==(TextNote a, TextNote b)
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

		public static bool operator !=(TextNote a, TextNote b) => !(a == b);
	}
}