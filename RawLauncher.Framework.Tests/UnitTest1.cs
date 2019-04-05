using Microsoft.VisualStudio.TestTools.UnitTesting;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var v1 = ModVersion.Parse("1.2.0.1-beta1");
            var v2= ModVersion.Parse("1.2.0.1");

            var flag = v1 < v2;

            Assert.AreEqual(true, flag);
        }
    }
}
