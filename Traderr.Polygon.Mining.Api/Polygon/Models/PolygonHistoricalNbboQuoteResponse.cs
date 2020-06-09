using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Traderr.Polygon.Mining.Api.Polygon.Models
{
    public class PolygonHistoricalNbboQuoteResponse
    {
        [JsonPropertyName("results_count")]
        public long Count { get; set; }

        [JsonPropertyName("db_latency")]
        public long DbLatency { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("results")]
        public List<PolygonHistoricalNbboQuoteModel> Results { get; set; }
    }
}