using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using Web2Sharp;
using Web2Sharp.Templates;
using System.IO;
using System.Collections.Generic;

namespace Web2Sharp.Tests
{
    [TestClass]
    public class Templates
    {
        [TestMethod]
        public void Templates_Basic()
        {
            var input = "Hello World!";
            var render = Template.FromString(input);
            var output = render(null);
            Assert.AreEqual(input, output);
        }


        [TestMethod]
        public void Templates_Encoding()
        {
            var input1 = "Some special chars: ☕☳üß - \n \r\n .,;(){}%$!\"";
            var render1 = Template.FromString(input1);
            var output1 = render1(null);
            Assert.AreEqual(input1, output1);

            var input2 = File.ReadAllText("Templates/Utf8Test.txt");
            Assert.IsTrue(input2.Contains("UTF-8 decoder capability and stress test"));
            Assert.IsTrue(input2.Contains("κόσμε"));
            var render2 = Template.FromString(input2);

            var output2 = render2(null);
            Assert.AreEqual(input2, output2);
        }

        [TestMethod]
        public void Templates_Values()
        {
            var input = "{% context Web2Sharp.Tests.TestContext,Web2Sharp.Tests %}SomeString: {{ SomeString }}, SomeInteger: {{ SomeInteger }}";
            var context = new TestContext
            {
                SomeString = "Hello World!",
                SomeInteger = 12345,
            };

            var render = Template.FromString(input);
            var output = render(context);
            Assert.AreEqual(output, "SomeString: Hello World!, SomeInteger: 12345");
        }

        [TestMethod]
        public void Templates_Empty_Template()
        {
            var input = "";

            var render = Template.FromString(input);
            var output = render(null);

            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void Templates_Files()
        {
            var context = new TestContext
            {
                SomeString = "Hello World!",
                SomeInteger = 12345,
            };

            string[] testFiles =
            {
                "Templates/TestFiles/Template1",
                "Templates/TestFiles/Template2",
                "Templates/TestFiles/Template3",
                "Templates/TestFiles/TemplateBlockBase",
                "Templates/TestFiles/TemplateBlockChild",
                "Templates/TestFiles/WeirdNameäö☕, ... !",
            };

            // Load templates multiple time to force the cache to become effective
            for (int i = 0; i < 3; i++)
            {
                foreach (var testFilename in testFiles)
                {
                    var expectedOutput = File.ReadAllText(testFilename + ".output");
                    var render = Template.FromFile(testFilename + ".input");
                    var output = render(context);

                    Assert.AreEqual(expectedOutput, output);
                }
            }
        }

        [TestMethod]
        public void Templates_Errors()
        {
            var brokenTemplates = new string[]
            {
                "{%",
                "{{",
                "{% if %}",
                "{% if true %}",
                "{% if null %}{%endif%}",
                "{{doesnotexist}}",
                "{% context ThisTypeDoesNotExist,InvalidAssembly %}",
                "{% using InvalidNamespace %}",
            };

            foreach(var input in brokenTemplates)
            {
                TemplateParserException catchedError = null;
                try
                {
                    var render = Template.FromString(input);
                }
                catch(TemplateParserException e)
                {
                    catchedError = e;
                }

                Assert.IsNotNull(catchedError);
            }

            IOException ioException = null;
            try
            {
                Template.FromFile("ThisFileDoesNotExist");
            }
            catch(IOException e)
            {
                ioException = e;
            }

            Assert.IsNotNull(ioException);

        }
    }

    public class TestContext
    {
        public string SomeString = "Hello World!";
        public int SomeInteger = 12345;
        public bool FlagTrue = true;
        public bool FlagFalse = false;
        public bool FlagProperty { get { return FlagTrue; } }
        public List<string> SomeList = new List<string> { "one", "two", "three" };
        public static string StaticString = "Lorem Ipsum";
    }
}
