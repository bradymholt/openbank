using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenBank.Test
{
    [TestClass]
    public class OfxParserTest
    {
        [TestMethod]
        public void ParseStatement()
        {
            string raw = File.ReadAllText("files\\BANKMSGSRSV1.ofx");
            OfxParser parser = new OfxParser();
            OfxResponse response = parser.Parse(raw);
            Assert.AreEqual(2703.71M, response.statement.ledger_balance.amount);
            
        }

        [TestMethod]
        public void ParseOfx()
        {
            string raw = File.ReadAllText("files\\ACCTINFORS.ofx");
            OfxParser parser = new OfxParser();
            OfxResponse response = parser.Parse(raw);
            Assert.AreEqual("CHECKING", response.accounts[0].account_type);
            Assert.AreEqual("MYACCESS CHECKING", response.accounts[0].description);

        }
    }
}
