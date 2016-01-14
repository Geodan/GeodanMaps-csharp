using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class IntersectFeature
    {
        [JsonProperty(PropertyName = "datasetName")]
        public string DatasetName { get; set; }

        [JsonProperty(PropertyName = "features")]
        public List<Feature> Features { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}
