using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RailPhase.Tests.Requests
{
    [TestFixture]
    class Requests
    {
        [Test]
        public void Requests_SimpleRequest()
        {
            var expectedResponse = "Hello World";

            var app = new App();
            app.AddUrl("/", (c) => expectedResponse);

            TestUtils.AppTest(app, () =>
            {
                var client = new WebClient();
                var actualResponse = client.DownloadString(TestUtils.AppPrefix);
                Assert.AreEqual(expectedResponse, actualResponse);
            });
        }

        [Test]
        public void Requests_ParallelRequests()
        {
            var expectedResponse = "Hello World";

            var app = new App();
            app.AddUrl("/", (c) => expectedResponse);

            Action InnerTest = () =>
            {
                var clientThreads = new Thread[10];
                for (int i = 0; i < 10; i++)
                {
                    clientThreads[i] = new Thread(() =>
                    {
                        var client = new WebClient();
                        var actualResponse = client.DownloadString(TestUtils.AppPrefix);
                        Assert.AreEqual(expectedResponse, actualResponse);
                    });
                }

                for (int i = 0; i < 10; i++)
                    clientThreads[i].Start();
                for (int i = 0; i < 10; i++)
                    clientThreads[i].Join();
            };

            

            // First, test without async request processing
            app.EnableAsyncProcessing = false;
            TestUtils.AppTest(app, InnerTest);

            app.EnableAsyncProcessing = true;
            TestUtils.AppTest(app, InnerTest);
        }
    }
}
