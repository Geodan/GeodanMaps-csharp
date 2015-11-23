using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class IntersectResult
    {
        [JsonProperty(PropertyName = "datasets")]
        public Dataset[] Datasets{ get; set; }
    }
}
