using NUnit.Framework;
using SoftAPIClient.Core;
using System;

namespace SoftAPIClient.Tests
{
    public class DynamicRequestSettingsTests : AbstractTest
    {
        [Test]
        public void VerifyRequestSettings()
        {
            var followRedirects = false;
            Func<string, string> encoder = s => s;
            var dynamicRequestSettings = new DynamicRequestSettings 
            { 
                FollowRedirects = followRedirects,
                Encoder = encoder
            };

            Assert.AreEqual(followRedirects, dynamicRequestSettings.FollowRedirects);
            Assert.AreEqual(encoder, dynamicRequestSettings.Encoder);
            Assert.AreEqual($"FollowRedirects={followRedirects}", dynamicRequestSettings.ToString());
        }

        [Test]
        public void VerifyDefaultRequestSettings()
        {
            var followRedirects = true;
            var dynamicRequestSettings = new DynamicRequestSettings();

            Assert.AreEqual(followRedirects, dynamicRequestSettings.FollowRedirects);
            Assert.IsNull(dynamicRequestSettings.Encoder);
        }
    }
}
