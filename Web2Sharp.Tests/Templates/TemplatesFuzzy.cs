using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Globalization;


using Web2Sharp;
using Web2Sharp.Templates;

namespace Web2Sharp.Tests
{
    [TestClass]
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

        [TestMethod]
        public void Templates_RandomInput()
        {
            for (int i = 0; i < 50; i++)
            {
                var input = generateRandomString(1024);
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
