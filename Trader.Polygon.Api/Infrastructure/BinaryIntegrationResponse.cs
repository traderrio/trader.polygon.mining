using Newtonsoft.Json;

namespace Trader.Polygon.Api.Infrastructure
{
    public class BinaryIntegrationResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public byte[] Result { get; set; }
        
        public static BinaryIntegrationResponse Ok(byte[] result)
        {
            return new BinaryIntegrationResponse
            {
                Result = result,
                StatusCode = 200,
                Success = true
            };
        }
        
        public static BinaryIntegrationResponse BadRequest(string errorMessage)
        {
            return new BinaryIntegrationResponse
            {
                Message = errorMessage,
                StatusCode = 400,
                Success = false
            };
        }
    }
}