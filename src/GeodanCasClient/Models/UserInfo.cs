using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Core.Models
{
    public class UserInfo
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "Organisation")]
        public string Organisation { get; set; }

        [JsonProperty(PropertyName = "OrganisationCode")]
        public string OrganisationCode { get; set; }

        [JsonProperty(PropertyName = "OrganisationID")]
        public string OrganisationId { get; set; }
    }

}
