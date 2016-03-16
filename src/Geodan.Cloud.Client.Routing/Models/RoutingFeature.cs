using System.Collections.Generic;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace Geodan.Cloud.Client.Routing.Models
{
    public class RoutingFeature : Feature
    {
        public RoutingFeature(IGeometryObject geometry, Dictionary<string, object> properties = null, string id = null) : base(geometry, properties, id)
        {
        }

        public RoutingFeature(IGeometryObject geometry, object properties, string id = null) : base(geometry, properties, id)
        {
        }

        public bool TryGetDistance(out double distance)
        {
            if (!Properties.ContainsKey("distance"))
            {
                distance = double.NaN;
                return false;
            }

            distance = (double)Properties["distance"];

            return true;
        }

        public bool TryGetDuration(out double duration)
        {
            if (!Properties.ContainsKey("duration"))
            {
                duration = double.NaN;
                return false;
            }

            duration = (double)Properties["duration"];

            return true;
        }
    }
}
