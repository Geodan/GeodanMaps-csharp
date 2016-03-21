using System.Collections.Generic;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing.Models
{
    public class RoutingFeatureCollection : FeatureCollection
    {
        //public new List<RoutingFeature> Features;

        [JsonProperty(PropertyName = "features", Required = Required.Always)]
        public new List<RoutingFeature> Features { get; private set; }
    }
}
