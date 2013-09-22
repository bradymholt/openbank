using System;
using System.IO;
using NUnit.Framework;
using OpenBank.Service;

namespace OpenBank.Test
{
	[TestFixture()]
	public class ParameterHelperTest
    {
        [Test]
        public void ParseDateParameterValid()
        {
            string validDate = "20130901";
			DateTime parsedDate = Service.ParameterHelper.GetDateParameter(validDate);
            Assert.AreEqual(validDate, parsedDate.ToString("yyyyMMdd"));
        }

        [Test]
        public void ParseDateParameterInvalid()
        {
            string validDate = "2011";
			DateTime parsedDate = Service.ParameterHelper.GetDateParameter(validDate);
            Assert.AreEqual(DateTime.Now.ToString("yyyyMMdd"), parsedDate.ToString("yyyyMMdd"));
        }
    }
}
