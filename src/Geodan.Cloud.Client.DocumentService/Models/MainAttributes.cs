using Newtonsoft.Json;

namespace Geodan.Cloud.Client.DocumentService.Models
{
    public class MainAttributes
    {
        [JsonProperty(PropertyName = "Implementation-Title")]
        public object ImplementationTitle { get; set; }

        [JsonProperty(PropertyName = "Implementation-Version")]
        public object ImplementationVersion { get; set; }

        [JsonProperty(PropertyName = "Implementation-Vendor-Id")]
        public object ImplementationVendorId { get; set; }

        [JsonProperty(PropertyName = "Build-Jdk")]
        public object BuildJdk { get; set; }

        [JsonProperty(PropertyName = "Built-By")]
        public object BuiltBy { get; set; }

        [JsonProperty(PropertyName = "Manifest-Version")]
        public object ManifestVersion { get; set; }

        [JsonProperty(PropertyName = "Created-By")]
        public object CreatedBy { get; set; }

        [JsonProperty(PropertyName = "Implementation-Build")]
        public object ImplementationBuild { get; set; }

        [JsonProperty(PropertyName = "Archiver-Version")]
        public object ArchiverVersion { get; set; }
    }
}
