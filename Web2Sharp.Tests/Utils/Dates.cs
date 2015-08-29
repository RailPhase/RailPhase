using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;

using NUnit.Framework;
using Web2Sharp;

namespace Web2Sharp.Tests.Utils
{
    [TestFixture]
    class Dates
    {
        [SetUp]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [Test]
        public void Utils_RFC822String()
        {
            var date = new DateTime(2015, 08, 29, 23, 01, 45, DateTimeKind.Utc);
            var result = date.ToRFC822String();
            Assert.AreEqual("Sat, 29 Aug 2015 23:01:45", result);
        }

        [Test]
        public void Utils_HumanDate()
        {
            var date = new DateTime(2015, 08, 29, 23, 01, 45, DateTimeKind.Utc);
            var result = date.ToHumanDate();
            Assert.AreEqual("Saturday, 29 August 2015", result);
        }
    }
}
