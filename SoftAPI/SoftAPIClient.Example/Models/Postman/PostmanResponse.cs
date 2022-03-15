using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SoftAPIClient.Example.Models.Postman
{
    public class PostmanResponse : AbstractJsonModel
    {
        [JsonProperty("args")]
        public IDictionary<string, string> Args { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("files")]
        public IDictionary<string, string> Files { get; set; }
        [JsonProperty("form")]
        public IDictionary<string, string> Form { get; set; }
        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; }
        [JsonProperty("json")]
        public object Json { get; set; }
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}
