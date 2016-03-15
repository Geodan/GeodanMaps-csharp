using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class Feature
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public string Properties { get; set; }

        [JsonProperty(PropertyName = "geometry")]
        public string Geometry { get; set; }
    }
}
