using System;
using Serilog.Events;
using Trader.Common;

namespace Trader.Polygon.Api.Common
{
    public static class LoggerFilters
    {
        private static readonly TimeSpan StartTime = new TimeSpan(3, 45, 0);
        private static readonly TimeSpan EndTime = new TimeSpan(20, 1, 0);

        public static bool Time(LogEvent arg)
        {
            var date = arg.Timestamp.UtcDateTime.ToMarketTime();
            if (date.TimeOfDay > EndTime || date.TimeOfDay < StartTime)
            {
                return false;
            }

            return true;
        }
    }
}