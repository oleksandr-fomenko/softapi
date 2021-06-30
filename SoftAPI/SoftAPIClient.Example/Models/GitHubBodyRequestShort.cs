using Newtonsoft.Json;

namespace SoftAPIClient.Example.Models
{
    public class GitHubBodyRequestShort : AbstractJsonModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
