using System;
using System.Collections.Generic;

namespace NotetasticApi.Notes
{
	public class CheckItem
	{
		public bool Checked { get; set; }
		public string Text { get; set; }
	}
	public class Checklist : Note
	{
		public List<CheckItem> Items { get; set; }

		public override bool IsValid =>
			base.IsValid && Items != null && !Items.Exists(_ => _.Text == null);

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			var other = (Checklist)obj;
			if (base.Equals(other))
			{
				if (Items == null || other.Items == null)
				{
					return Items == null && other.Items == null;
				}
				if (Items.Count != other.Items.Count)
				{
					return false;
				}
				for (int i = 0; i < Items.Count; i++)
				{
					var a = Items[i];
					var b = other.Items[i];
					if (a.Text != b.Text || a.Checked != b.Checked)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, NBID, Archived, Title).GetHashCode();
		}

		public static bool operator ==(Checklist a, Checklist b)
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

		public static bool operator !=(Checklist a, Checklist b) => !(a == b);
	}
}