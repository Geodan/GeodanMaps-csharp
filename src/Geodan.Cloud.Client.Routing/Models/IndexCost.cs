using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing.Models
{
    public class IndexCost
    {
        [JsonProperty(PropertyName = "OriginalIndex")]
        public int OriginalIndex { get; set; }

        [JsonProperty(PropertyName = "SortedIndex")]
        public int SortedIndex { get; set; }

        [JsonProperty(PropertyName = "Cost")]
        public double Cost { get; set; }
    }
}
