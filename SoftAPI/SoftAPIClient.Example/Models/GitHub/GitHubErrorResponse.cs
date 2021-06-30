using System;
using Newtonsoft.Json;

namespace SoftAPIClient.Example.Models.GitHub
{
    public class GitHubErrorResponse : AbstractJsonModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("documentation_url")]
        public Uri DocumentationUrl { get; set; }
    }
}
