using System;
using System.Collections.Generic;
using System.Linq;

namespace NotetasticApi.Notes
{
	public class NotebookItem
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
			var other = (NotebookItem)obj;
			return Id == other.Id && Title == other.Title && Type == other.Type;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return Tuple.Create(Id, Title, Type).GetHashCode();
		}

		public static bool operator ==(NotebookItem a, NotebookItem b)
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

		public static bool operator !=(NotebookItem a, NotebookItem b) => !(a == b);
	}

	public class Notebook : Note
	{
		public bool IsRoot { get; set; }
		public List<NotebookItem> Items { get; set; }

		public override bool IsValid => base.IsValid;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (Notebook)obj;
			return base.Equals(other) && IsRoot == other.IsRoot && (
				(Items == null && other.Items == null) ||
				(Items != null && other.Items != null && Items.SequenceEqual(other.Items))
			);
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, NBID, Archived, Title).GetHashCode();
		}

		public static bool operator ==(Notebook a, Notebook b)
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

		public static bool operator !=(Notebook a, Notebook b) => !(a == b);
	}
}