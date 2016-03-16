using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing.Models
{
    public class TspResponse
    {
        [JsonProperty(PropertyName = "Identifier")]
        public int Identifier { get; set; }

        [JsonProperty(PropertyName = "TotalCost")]
        public double TotalCost { get; set; }

        [JsonProperty(PropertyName = "IndexCosts")]
        public List<IndexCost> IndexCosts { get; set; }
    }
}
