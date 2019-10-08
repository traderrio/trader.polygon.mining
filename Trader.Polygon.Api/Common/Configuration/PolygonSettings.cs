namespace Trader.Polygon.Api.Common.Configuration
{
    public class PolygonSettings
    {
        public string ApiKey { get; set; }
        public string StocksUrl { get; set; }
        public string ForexUrl { get; set; }
        public string CryptoUrl { get; set; }
        public DataSaverType SaverType { get; set; }
    }
}