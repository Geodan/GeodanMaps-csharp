using System.IO;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.DocumentService.Models
{
    public class MultipartFile
    {
        [JsonProperty(PropertyName = "originalFilename")]
        public string Filename { get; set; }

        [JsonProperty(PropertyName = "inputStream")]
        public Stream Data { get; set; }
    }
}
