using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing.RequestParams
{
    public class BatchLocation
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "FromCoordX")]
        public double FromCoordX { get; set; }

        [JsonProperty(PropertyName = "FromCoordY")]
        public double FromCoordY { get; set; }

        [JsonProperty(PropertyName = "ToCoordX")]
        public double ToCoordX { get; set; }

        [JsonProperty(PropertyName = "ToCoordY")]
        public double ToCoordY { get; set; }
    }
}
