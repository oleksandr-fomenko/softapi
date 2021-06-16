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
            const bool followRedirects = false;
            string Encoder(string s) => s;
            var dynamicRequestSettings = new DynamicRequestSettings 
            { 
                FollowRedirects = followRedirects,
                Encoder = Encoder
            };

            Assert.AreEqual(followRedirects, dynamicRequestSettings.FollowRedirects);
            Assert.AreEqual((Func<string, string>) Encoder, dynamicRequestSettings.Encoder);
            Assert.AreEqual($"FollowRedirects={followRedirects}", dynamicRequestSettings.ToString());
        }

        [Test]
        public void VerifyDefaultRequestSettings()
        {
            const bool followRedirects = true;
            var dynamicRequestSettings = new DynamicRequestSettings();

            Assert.AreEqual(followRedirects, dynamicRequestSettings.FollowRedirects);
            Assert.IsNull(dynamicRequestSettings.Encoder);
        }
    }
}
