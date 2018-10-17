using System;

namespace NotetasticApi.Notes
{
	public class Location : Note
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public override bool IsValid =>
			UID != null && Title != null && Math.Abs(Latitude) <= 90 && Math.Abs(Longitude) <= 180;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var other = (Location)obj;
			return base.Equals(other) && Latitude == other.Latitude && Longitude == other.Longitude;
		}

		public override int GetHashCode()
		{
			return Tuple.Create(Id, UID, NBID, Archived, Title, Latitude, Longitude).GetHashCode();
		}

		public static bool operator ==(Location a, Location b)
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

		public static bool operator !=(Location a, Location b) => !(a == b);
	}
}