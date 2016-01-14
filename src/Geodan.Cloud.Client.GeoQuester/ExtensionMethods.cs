using System;

namespace Geodan.Cloud.Client.GeoQuester
{
    public static class ExtensionMethods
    {
        public static string GetStringValue(this IntersectOutputProperties value)
        {            
            switch (value)
            {
                case IntersectOutputProperties.All:
                    return "all";
                case IntersectOutputProperties.AllExceptGeometry:
                    return "all-except-geometry";
                case IntersectOutputProperties.Configuration:
                    return "configuration";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
