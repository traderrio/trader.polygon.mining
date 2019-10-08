using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Trader.Polygon.Core
{
	//TODO move to infrastructure api
	public class EpochInMillisecondsJsonConverter : DateTimeConverterBase
	{
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteRawValue(string.Empty);
				return;
			}
			
			var timestamp = (long)((DateTime)value - Epoch).TotalMilliseconds;
			writer.WriteRawValue(timestamp.ToString(CultureInfo.InvariantCulture));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
			{
				return null;
			}

			return Epoch.AddMilliseconds((long)reader.Value);
		}
	}
}
