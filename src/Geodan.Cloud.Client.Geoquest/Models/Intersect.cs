using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class Intersect
    {
        [JsonProperty(PropertyName = "datasetName")]
        public string DatasetName { get; set; }

        [JsonProperty(PropertyName = "featureFound")]
        public bool FeatureFound { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}
