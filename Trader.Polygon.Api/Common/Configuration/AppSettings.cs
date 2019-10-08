using Trader.Common.Configuration;

namespace Trader.Polygon.Api.Common.Configuration
{
    public class AppSettings
    {
	    public PolygonSettings Polygon{ get; set; }
	    public RedisSettings Redis{ get; set; }
	    public DataProcessingSettings DataProcessing { get; set; }
        public MicroservicesSettings Microservices { get; set; }
    }
}
