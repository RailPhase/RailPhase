using System;
using NUnit.Framework;
using System.Text;
using System.Globalization;


using Web2Sharp;
using Web2Sharp.Templates;

namespace Web2Sharp.Tests
{
    [TestFixture]
    public class TemplatesFuzzy
    {
        static Random rand = new Random();
        string generateRandomString(int length)
        {
            var s = new StringBuilder();
            
            for(int i = 0; i < length; i++)
            {
                char c = (char)rand.Next(65536);
                s.Append(c.ToString());
            }

            return s.ToString();
        }

        [Test]
        public void Templates_RandomInput()
        {
            for (int i = 0; i < 10; i++)
            {
                var input = generateRandomString(4096);
                if (!input.Contains("{{") && !input.Contains("{%"))
                {
                    var render = Template.FromString(input);
                    var output = render(null);
                    Assert.AreEqual(input, output);
                }
            }
        }
    }
}
