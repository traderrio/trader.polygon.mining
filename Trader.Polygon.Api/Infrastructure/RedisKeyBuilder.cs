using System;

namespace Trader.Polygon.Api.Infrastructure
{
    public static class RedisKeyBuilder
    {
        public static string LastTradesPrefix => "last-trades";
        
        public static string GetLastTradesKey(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
            {
                throw new ArgumentNullException(nameof(ticker));
            }

            return $"{LastTradesPrefix}:{ticker}";
        }
    }
}