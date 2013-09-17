using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenBank.Test
{
    [TestClass]
    public class AccountList
    {
        [TestMethod]
        public void ParseOfx()
        {
            string raw = File.ReadAllText("files\\ACCTINFORS.ofx");
            OfxParser parser = new OfxParser();
            OfxResponse response = parser.Parse(raw);
            Assert.AreEqual("CHECKING", response.Account.Accounts[0].AccountType);
            Assert.AreEqual("MYACCESS CHECKING", response.Account.Accounts[0].Description);
            
        }
    }
}
