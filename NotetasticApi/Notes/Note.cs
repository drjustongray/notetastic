using System;
using NotetasticApi.Common;

namespace NotetasticApi.Notes
{
	public abstract class Note : Record
	{
		/// <summary>
		/// The id of the user that owns this note
		/// </summary>
		/// <value></value>
		public string UID { get; set; }
		/// <summary>
		/// the id of the notebook this note belongs in
		/// </summary>
		/// <value></value>
		public string NBID { get; set; }
		public bool Archived { get; set; }
		public string Title { get; set; }
		public abstract bool IsValid { get; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			var other = obj as Note;

			if (other == null)
			{
				return false;
			}

			return Id == other.Id &&
				UID == other.UID &&
				NBID == other.NBID &&
				Archived == other.Archived &&
				Title == other.Title;
		}
		
		// override object.GetHashCode
		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, NBID, Archived, Title).GetHashCode();
		}
	}
}