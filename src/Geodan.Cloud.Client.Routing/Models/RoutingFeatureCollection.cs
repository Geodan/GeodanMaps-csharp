using System.Collections.Generic;
using GeoJSON.Net.Feature;

namespace Geodan.Cloud.Client.Routing.Models
{
    public class RoutingFeatureCollection : FeatureCollection
    {
        public new List<RoutingFeature> Features { get; set; }
    }
}
