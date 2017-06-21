using System;
using NUnit.Framework;
using System.Text;
using System.Globalization;


using RailPhase;

namespace RailPhase.Tests
{
    [TestFixture]
    public class TemplatesFuzzy
    {
 
        [Test]
        public void Templates_RandomInput()
        {
            for (int i = 0; i < 10; i++)
            {
                var input = TestUtils.GenerateRandomString(4096);
                if (!input.Contains("{{") && !input.Contains("{%"))
                {
                    var render = Template.FromString(input);
                    var output = render(null, null);
                    Assert.AreEqual(input, output);
                }
            }
        }
    }
}
