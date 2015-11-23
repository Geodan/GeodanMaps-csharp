using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class Dataset
    {
        [JsonProperty(PropertyName = "datasetName")]
        public string DatasetName { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "featureTypeName")]
        public string FeatureTypeName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public string[] Properties { get; set; }

        [JsonProperty(PropertyName = "featureFound")]
        public bool FeatureFound { get; set; }
    }
}
