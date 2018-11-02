using System;

namespace NotetasticApi.Common
{
	public interface ITimeService
	{
		DateTimeOffset GetCurrentTime();
	}
	public class TimeService : ITimeService
	{
		public DateTimeOffset GetCurrentTime()
		{
			return DateTimeOffset.Now;
		}
	}
}