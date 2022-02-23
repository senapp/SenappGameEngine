using System.Net;

using Newtonsoft.Json;

namespace Senapp.Engine.Networking.Json
{
    [JsonObject]
    public class NetworkPacket
    {
        [JsonProperty]
        public HttpStatusCode StatusCode;
        [JsonProperty]
        public string Type;
        [JsonProperty]
        public string Data;
        [JsonProperty]
        public string Message;

        public NetworkPacket(HttpStatusCode success, string type, string data, string message)
        {
            this.StatusCode = success;
            this.Type = type;
            this.Data = data;
            this.Message = message;
        }

        public static NetworkPacket CreateRequest(string type, string data) => new(HttpStatusCode.OK, type, data, string.Empty);

        public static NetworkPacket Invalid => new(HttpStatusCode.NotFound, string.Empty, string.Empty, string.Empty);
        public override string ToString()
        {
            return $"Success: {StatusCode}, Type: {Type}, Data: {Data}, Message: {Message}";
        }

        public NetworkPacket Optimize()
        {
            Message = string.Empty;
            return this;
        }

        public bool IsSuccessStatusCode
        {
            get { return ((int)StatusCode >= 200) && ((int)StatusCode <= 299); }
        }
    }
}
