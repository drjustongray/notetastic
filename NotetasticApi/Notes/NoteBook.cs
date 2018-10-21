using System;
using System.Collections.Generic;
using System.Linq;

namespace NotetasticApi.Notes
{
	public class NoteBookItem
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Type { get; set; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (NoteBookItem)obj;
			return Id == other.Id && Title == other.Title && Type == other.Type;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return Tuple.Create(Id, Title, Type).GetHashCode();
		}

		public static bool operator ==(NoteBookItem a, NoteBookItem b)
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

		public static bool operator !=(NoteBookItem a, NoteBookItem b) => !(a == b);
	}

	public class NoteBook : Note
	{
		public List<NoteBookItem> Items { get; set; }

		public override bool IsValid => base.IsValid;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (NoteBook)obj;
			return base.Equals(other) && (
				(Items == null && other.Items == null) ||
				(Items != null && other.Items != null && Items.SequenceEqual(other.Items))
			);
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, NBID, Archived, Title, Items).GetHashCode();
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