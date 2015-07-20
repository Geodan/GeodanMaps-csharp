using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class IsOverlapResult
    {
        /// <summary>
        /// States if the feature intersects with layer shape
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public bool IsOverlap { get; set; }
    }
}
