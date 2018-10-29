using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace NotetasticApi.Notes
{
	public class NoteMetaData
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
			var other = (NoteMetaData)obj;
			return Id == other.Id && Title == other.Title && Type == other.Type;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return Tuple.Create(Id, Title, Type).GetHashCode();
		}

		public static bool operator ==(NoteMetaData a, NoteMetaData b)
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

		public static bool operator !=(NoteMetaData a, NoteMetaData b) => !(a == b);
	}
}