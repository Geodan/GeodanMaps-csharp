using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing.RequestParams
{
    public class TspLocation
    {
        [JsonProperty(PropertyName = "CoordX")]
        public double CoordX { get; set; }

        [JsonProperty(PropertyName = "CoordY")]
        public double CoordY { get; set; }
    }
}
