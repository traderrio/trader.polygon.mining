using System;
using System.Text.Json.Serialization;
using Traderr.Polygon.Mining.Api.Core;

namespace Traderr.Polygon.Mining.Api.Polygon.Models
{
    public class PolygonHistoricalNbboQuoteModel
    {
        public string Ticker { get; set; }

        [JsonPropertyName("t")]
        [JsonConverter(typeof (EpochInNanosecondsJsonConverter))]
        public DateTime? SipDateTime { get; set; }

        [JsonPropertyName("y")]
        [JsonConverter(typeof (EpochInNanosecondsJsonConverter))]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("f")]
        [JsonConverter(typeof (EpochInNanosecondsJsonConverter))]
        public DateTime? TrfDateTime { get; set; }

        [JsonPropertyName("q")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("p")]
        public Decimal? BidPrice { get; set; }

        [JsonPropertyName("x")]
        public int? BidExchange { get; set; }

        [JsonPropertyName("s")]
        public long? BidSize { get; set; }

        [JsonPropertyName("P")]
        public Decimal? AskPrice { get; set; }

        [JsonPropertyName("X")]
        public int? AskExchange { get; set; }

        [JsonPropertyName("S")]
        public long? AskSize { get; set; }

        [JsonPropertyName("z")]
        public int? Tape { get; set; }
    }
}