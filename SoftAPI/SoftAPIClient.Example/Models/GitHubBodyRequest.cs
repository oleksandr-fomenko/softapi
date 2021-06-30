using Newtonsoft.Json;

namespace SoftAPIClient.Example.Models
{
    public class GitHubBodyRequest : AbstractJsonModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("organization_permission")]
        public string OrganizationPermission { get; set; }

        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }
}
