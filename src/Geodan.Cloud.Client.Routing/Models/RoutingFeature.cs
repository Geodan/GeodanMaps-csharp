using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing.Models
{
    public class RoutingFeature : Feature
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GeoJSON.Net.Feature.Feature"/> class.
        /// 
        /// </summary>
        /// <param name="geometry">The Geometry Object.</param><param name="properties">The properties.</param><param name="id">The (optional) identifier.</param>
        [JsonConstructor]
        public RoutingFeature(IGeometryObject geometry, Dictionary<string, object> properties = null, string id = null) : base(geometry, properties, id){}
        public RoutingFeature(IGeometryObject geometry, object properties, string id = null) : base(geometry, properties, id) {}

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
