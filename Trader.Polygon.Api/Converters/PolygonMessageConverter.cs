using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trader.Polygon.Api.StreamMessages;
using Trader.Polygon.Core.Common.Enums;
using Trader.Polygon.Core.Streaming.Messages;
using Trader.Polygon.Core.Streaming.Messages.Stocks;

namespace Trader.Polygon.Api.Converters
{
    public class PolygonMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var messages = new List<StreamingMessage>();
            var jObject = JToken.ReadFrom(reader);
            if (jObject is JArray)
            {
                foreach (var jToken in jObject)
                {
                    var message = ParseMessage(serializer, jToken);
                    if (message != null)
                    {
                        messages.Add(message);
                    }
                }
            }
            else
            {
                throw new Exception("Need to support single object also");
            }

            return messages;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StreamingMessage) ||
                   objectType == typeof(IList<StreamingMessage>);
        }

        private StreamingMessage ParseMessage(JsonSerializer serializer, JToken jToken)
        {
            var typeString = jToken["ev"].Value<string>();
            var type = ParseType(typeString);

            StreamingMessage result;
            switch (type)
            {
                case StreamingMessageType.Status:
                    result = new StreamingStatusMessage();
                    break;
                case StreamingMessageType.StockLastTrade:
                    result = new StockLastTradeMessage();
                    break;
                case StreamingMessageType.StockLastQuote:
                    result = new StockLastQuoteMessage();
                    break;
                case StreamingMessageType.StockSecondAggregated:
                case StreamingMessageType.StockMinuteAggregated:
                    result = new StockSecondAggregatedMessage();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            serializer.Populate(jToken.CreateReader(), result);

            return result;
        }

        private StreamingMessageType ParseType(string enumString)
        {
            if (string.IsNullOrWhiteSpace(enumString))
            {
                return StreamingMessageType.None;
            }

            switch (enumString)
            {
                case "status":
                    return StreamingMessageType.Status;
                case "T":
                    return StreamingMessageType.StockLastTrade;
                case "Q":
                    return StreamingMessageType.StockLastQuote;
                case "A":
                    return StreamingMessageType.StockSecondAggregated;
                case "AM":
                    return StreamingMessageType.StockMinuteAggregated;
                default:
                    return StreamingMessageType.None;
            }
        }
    }
}