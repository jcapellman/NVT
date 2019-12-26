using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NVT.lib;
using NVT.lib.Objects;

namespace NVT.UnitTests
{
    [TestClass]
    public class DBTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNullIPAddress()
        {
            DB.CheckDB(null);

            throw new AssertFailedException();
        }

        [TestMethod]
        public void GetNotFoundIPAddress()
        {
            var result = DB.CheckDB("127.256.0.1");

            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullToDB()
        {
            DB.AddToDB(null);

            throw new AssertFailedException();
        }

        [TestMethod]
        public void AddValidToDB()
        {
            var item = new NetworkConnectionItem
            {
                IPAddress = "256.256.256.256"
            };

            DB.AddToDB(item);

            var result = DB.CheckDB(item.IPAddress);

            Assert.IsNotNull(result);
        }
    }
}