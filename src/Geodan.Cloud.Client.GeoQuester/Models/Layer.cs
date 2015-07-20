using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geodan.Cloud.Client.GeoQuester.Models
{
    public class Layer
    {
        /// <summary>
        /// Name of the account
        /// </summary>
        [JsonProperty(PropertyName = "account")]
        public string Account { get; set; }

        /// <summary>
        /// Name of the Service
        /// </summary>
        [JsonProperty(PropertyName = "service")]
        public string Service { get; set; }

        /// <summary>
        /// Name of the layer
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }


        /// <summary>
        /// Id of the layer
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string LayerId { get; set; }

        /// <summary>
        /// Expiration date, empty for no expiration
        /// </summary>
        [JsonProperty(PropertyName = "expiration")]
        public string ExpirationDate { get; set; }

        /// <summary>
        /// Title of the layer
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Description of the layer
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Content type of the data
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// States if the document will be visible for other accounts
        /// </summary>
        [JsonProperty(PropertyName = "public")]
        public bool IsPublic { get; set; }
    }
}
