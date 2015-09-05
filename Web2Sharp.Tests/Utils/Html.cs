using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Web2Sharp.Tests.Utils
{
    [TestFixture]
    public class Html
    {
        [Test]
        public void Utils_StripHtmlTags()
        {
            var strippedTextByInput = new Dictionary<string, string>
            {
                {@"<h1>Lorem Ipsum</h1>", "Lorem Ipsum"},
                {@"<h1 style='color: red;'>Lorem <span color='blue'>Ipsum</span></h1>", "Lorem Ipsum"},
                {@"", ""},
            };

            foreach (var kvp in strippedTextByInput)
            {
                var html = kvp.Key;
                var expectedText = kvp.Value;

                var result = Web2Sharp.Utils.StripHtmlTags(html);
                Assert.AreEqual(expectedText, result);
            }
        }
    }
}
