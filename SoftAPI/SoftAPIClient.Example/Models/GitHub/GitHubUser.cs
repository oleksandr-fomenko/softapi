using System;
using Newtonsoft.Json;

namespace SoftAPIClient.Example.Models.GitHub
{
    public class GitHubUser : AbstractJsonModel
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("node_id")]
        public string NodeId { get; set; }
        [JsonProperty("avatar_url")]
        public Uri AvatarUrl { get; set; }
        [JsonProperty("gravatar_id")]
        public string GravatarId { get; set; }
        [JsonProperty("url")]
        public Uri Url { get; set; }
        [JsonProperty("html_url")]
        public Uri HtmlUrl { get; set; }
        [JsonProperty("followers_url")]
        public Uri FollowersUrl { get; set; }
        [JsonProperty("following_url")]
        public string FollowingUrl { get; set; }
        [JsonProperty("gists_url")]
        public string GistsUrl { get; set; }
        [JsonProperty("starred_url")]
        public string StarredUrl { get; set; }
        [JsonProperty("subscriptions_url")]
        public Uri SubscriptionsUrl { get; set; }
        [JsonProperty("organizations_url")]
        public Uri OrganizationsUrl { get; set; }
        [JsonProperty("repos_url")]
        public Uri ReposUrl { get; set; }
        [JsonProperty("events_url")]
        public string EventsUrl { get; set; }
        [JsonProperty("received_events_url")]
        public Uri ReceivedEventsUrl { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("site_admin")]
        public bool SiteAdmin { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("company")]
        public string Company { get; set; }
        [JsonProperty("blog")]
        public string Blog { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("email")]
        public object Email { get; set; }
        [JsonProperty("hireable")]
        public object Hireable { get; set; }
        [JsonProperty("bio")]
        public object Bio { get; set; }
        [JsonProperty("twitter_username")]
        public object TwitterUsername { get; set; }
        [JsonProperty("public_repos")]
        public long PublicRepos { get; set; }
        [JsonProperty("public_gists")]
        public long PublicGists { get; set; }
        [JsonProperty("followers")]
        public long Followers { get; set; }
        [JsonProperty("following")]
        public long Following { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
        [JsonProperty("private_gists")]
        public long PrivateGists { get; set; }
        [JsonProperty("total_private_repos")]
        public long TotalPrivateRepos { get; set; }
        [JsonProperty("owned_private_repos")]
        public long OwnedPrivateRepos { get; set; }
        [JsonProperty("disk_usage")]
        public long DiskUsage { get; set; }
        [JsonProperty("collaborators")]
        public long Collaborators { get; set; }
        [JsonProperty("two_factor_authentication")]
        public bool TwoFactorAuthentication { get; set; }
        [JsonProperty("plan")]
        public GitHubPlan Plan { get; set; }
    }
}
