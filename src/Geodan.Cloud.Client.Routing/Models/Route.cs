using System.Linq;
using Geodan.Cloud.Client.Routing.Extensions;
using Geodan.Cloud.Client.Routing.RequestParams;
using GeoJSON.Net.Feature;

namespace Geodan.Cloud.Client.Routing.Models
{
    public class Route
    {
        public TimeUnit TimeUnit { get; private set; }
        public DistanceUnit DistanceUnit { get; private set; }
        public RoutingFeatureCollection FeatureCollection { get; private set; }

        public Route(RoutingFeatureCollection featureCollection, UnitFormatType unitFormat)
        {
            TimeUnit = unitFormat.ToTimeUnit();
            DistanceUnit = unitFormat.ToDistanceUnit();
            FeatureCollection = featureCollection;
        }        
    }
}
