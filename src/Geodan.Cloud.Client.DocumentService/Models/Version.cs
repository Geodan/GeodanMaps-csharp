using Newtonsoft.Json;

namespace Geodan.Cloud.Client.DocumentService.Models
{
    public class Version
    {
        [JsonProperty(PropertyName = "entries")]
        public object Entries { get; set; }

        [JsonProperty(PropertyName = "mainAttributes")]
        public MainAttributes MainAttributes { get; set; }
    }
}
