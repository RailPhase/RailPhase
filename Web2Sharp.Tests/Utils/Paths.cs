using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Web2Sharp;

using NUnit.Framework;

namespace Web2Sharp.Tests.Utils
{
    [TestFixture]
    public class Paths
    {
        [Test]
        public void Utils_MakeRelativePath()
        {
            var referencePath = "/1/abc/def/";

            var relativePathByAbsoluteInput = new Dictionary<string, string>
            {
                ["/1/abc/def/"] = "",
                ["/1/"] = "../../",
                ["/1/abc/xyz"] = "../xyz",
                ["/1/abc/def/ghi"] = "ghi",
                ["/1/abc/def/ghi/"] = "ghi/",
                ["/2"] = "../../../2",
                [""] = "../../../"
            };

            foreach(var kvp in relativePathByAbsoluteInput)
            {
                var absolute = kvp.Key;
                var expectedResult = kvp.Value;

                var result = Web2Sharp.Utils.MakeRelativePath(absolute, referencePath);

                Assert.AreEqual(expectedResult, result);
            }
        }

        [Test]
        public void Utils_SystemToUnixPath()
        {
            var unixPathsByInput = new Dictionary<string, string>
            {
                [@"some\windows\path"] = "some/windows/path",
                [@"abc\..\abc\def\üöä\"] = "abc/../abc/def/üöä/",
                [@""] = "",
            };

            foreach(var kvp in unixPathsByInput)
            {
                var systemPath = kvp.Key;
                systemPath = systemPath.Replace('\\', System.IO.Path.DirectorySeparatorChar);
                var expectedUnixPath = kvp.Value;

                var result = Web2Sharp.Utils.SystemToUnixPath(systemPath);
                Assert.AreEqual(expectedUnixPath, result);
            }
        }
    }
}
