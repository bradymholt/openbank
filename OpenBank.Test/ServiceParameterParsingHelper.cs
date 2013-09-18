using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenBank.Test
{
    [TestClass]
    public class ServiceParameterParsingHelperTest
    {
        [TestMethod]
        public void ParseDateParameterValid()
        {
            string validDate = "20130901";
            DateTime parsedDate = ServiceParameterParsingHelper.GetDateParameter(validDate);
            Assert.AreEqual(validDate, parsedDate.ToString("yyyyMMdd"));
        }

        [TestMethod]
        public void ParseDateParameterInvalid()
        {
            string validDate = "2011";
            DateTime parsedDate = ServiceParameterParsingHelper.GetDateParameter(validDate);
            Assert.AreEqual(DateTime.Now.ToString("yyyyMMdd"), parsedDate.ToString("yyyyMMdd"));

        }
    }
}
