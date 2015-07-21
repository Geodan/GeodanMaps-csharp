using Newtonsoft.Json;

namespace Geodan.Cloud.Client.DocumentService.Models
{
    public class DataDocument
    {
        /// <summary>
        /// Name of the account
        /// </summary>
        [JsonProperty(PropertyName = "account")]
        public string Account { get; set; }

        /// <summary>
        /// Name of the account
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Name of the service
        /// </summary>
        [JsonProperty(PropertyName = "service")]
        public string Service { get; set; }

        /// <summary>
        /// Name of the document
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Title of the document
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Description of the document
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// Content type of the data
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Expiration date of this document in ISO-8601 format, empty for no expiration
        /// </summary>
        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }

        /// <summary>
        /// States if the document will be visible for other accounts
        /// </summary>
        [JsonProperty(PropertyName = "public")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Name of the encoder, will be guessed when not specified
        /// </summary>
        [JsonProperty(PropertyName = "encoder")]        
        public string Encoder { get; set; }
    }
}
