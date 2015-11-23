using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class Configuration
    {
        [JsonProperty(PropertyName = "configurationName")]
        public string ConfigurationName { get; set; }

        [JsonProperty(PropertyName = "datasets")]
        public Dataset[] Datasets { get; set; }
    }
}
