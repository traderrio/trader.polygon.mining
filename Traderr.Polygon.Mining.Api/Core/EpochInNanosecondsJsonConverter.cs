using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Traderr.Polygon.Mining.Api.Core
{
    public class EpochInNanosecondsJsonConverter : JsonConverterFactory
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return typeToConvert == typeof(DateTime?)
                ? (JsonConverter) new DateTimeNullableConverter()
                : new DateTimeConverter();
        }
        
        private class DateTimeConverter : JsonConverter<DateTime>
        {
            private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?);
            }
        
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.Number)
                {
                    throw new JsonException();
                }

                return Epoch.AddTicks(reader.GetInt64() / 100L);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString("c", CultureInfo.InvariantCulture));
        }

        private class DateTimeNullableConverter : JsonConverter<DateTime?>
        {
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.None || reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }
                
                if (reader.TokenType != JsonTokenType.Number)
                {
                    throw new JsonException($"Token type is not Number but {reader.TokenType} ");
                }
                
                return Epoch.AddTicks(reader.GetInt64() / 100L);
            }

            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
                => writer.WriteStringValue(value!.Value.ToString("c", CultureInfo.InvariantCulture));
        }
    }
}