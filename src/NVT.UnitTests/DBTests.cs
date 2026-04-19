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
        public void GetNotFoundIPAddress()
        {
            var item = new NetworkConnectionItem {IPAddress = "127.256.0.1"};

            var result = DB.CheckDB(ref item);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddNullToDB()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => DB.AddToDB(null!));
        }

        [TestMethod]
        public void AddValidToDB()
        {
            var item = new NetworkConnectionItem
            {
                IPAddress = "256.256.256.256"
            };

            DB.AddToDB(item);

            var result = DB.CheckDB(ref item);

            Assert.IsTrue(result);
        }
    }
}