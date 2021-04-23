using System;

namespace SoftAPIClient.Core
{
    public class DynamicRequestSettings
    {
        public bool FollowRedirects { get; set; } = true;
        public Func<string, string> Encoder { get; set; } = null;
        // other settings will be added by demand
        public override string ToString()
        {
            return $"FollowRedirects={FollowRedirects}";
        }
    }
}
