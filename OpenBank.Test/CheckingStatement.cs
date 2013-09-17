using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenBank.Test
{
    [TestClass]
    public class CheckingStatement
    {
        [TestMethod]
        public void ParseOfx()
        {
            string raw = File.ReadAllText("files\\BANKMSGSRSV1.ofx");
            OfxParser parser = new OfxParser();
            OfxResponse response = parser.Parse(raw);
            Assert.AreEqual(2703.71M, response.Statement.LedgerBalance.Amount);
            
        }
    }
}
