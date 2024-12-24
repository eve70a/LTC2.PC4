using Newtonsoft.Json;

namespace LTC2.Shared.StravaConnector.Models
{
    public class StravaRoute
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        public string RouteId => Id.ToString();

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}
