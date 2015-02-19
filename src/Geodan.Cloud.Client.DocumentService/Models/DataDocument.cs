using Newtonsoft.Json;

namespace Geodan.Cloud.Client.DocumentService.Models
{
    public class DataDocument
    {
        [JsonProperty(PropertyName = "account")]
        public string Account { get; set; }

        [JsonProperty(PropertyName = "service")]
        public string Service { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }

        [JsonProperty(PropertyName = "public")]
        public bool IsPublic { get; set; }

        [JsonProperty(PropertyName = "encoder")]
        public string Encoder { get; set; }
    }
}
