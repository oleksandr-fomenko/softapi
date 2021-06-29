using Newtonsoft.Json;

namespace SoftAPIClient.Example.Models
{
    public class GitHubPlan : AbstractJsonModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("space")]
        public long Space { get; set; }

        [JsonProperty("collaborators")]
        public long Collaborators { get; set; }

        [JsonProperty("private_repos")]
        public long PrivateRepos { get; set; }
    }
}
