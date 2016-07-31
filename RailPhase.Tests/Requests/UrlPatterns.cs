using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Threading;
using System.Net;
using System.IO;

namespace RailPhase.Tests.Requests
{
    [TestFixture]
    class UrlPatterns
    {


        [Test]
        public void UrlPatterns_ConstUrls()
        {
            // Generate many random strings and make sure they work correctly with the app.AddUrl view.
            var urls = new Dictionary<string, int>();

            var app = new App();

            for (int i = 0; i < 100; i++)
            {
                var randomString = TestUtils.GenerateRandomURL(100);
                if (!urls.ContainsKey(randomString))
                {
                    int response = i;
                    urls[randomString] = response;
                    app.AddStaticStringView(randomString, (c) => response.ToString());
                }
            }

            TestUtils.AppTest(app, () =>
            {
                var client = new WebClient();

                foreach (var url in urls.Keys)
                {
                    var expectedResult = urls[url].ToString();
                    var result = client.DownloadString(TestUtils.AppPrefix + url);
                    Assert.AreEqual(expectedResult, result);
                }
            });

        }
    }
}
