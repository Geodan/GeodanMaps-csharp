using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class IntersectFeatureResult
    {
        [JsonProperty(PropertyName = "datasets")]
        public List<IntersectFeature> Datasets { get; set; }
    }
}
