using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class IntersectResult
    {
        [JsonProperty(PropertyName = "datasets")]
        public List<Intersect> Datasets { get; set; }
    }
}
