using Newtonsoft.Json;

namespace SoftAPIClient.Example.Models
{
    public class GitHubBodyRequest : GitHubBodyRequestShort
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("organization_permission")]
        public string OrganizationPermission { get; set; }

        [JsonProperty("private")]
        public bool? IsPrivate { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }
}
