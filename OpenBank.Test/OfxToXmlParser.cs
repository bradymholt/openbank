using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace OpenBank.Test
{
	[TestFixture()]
    public class OfxToXmlParserTest
    {
        [Test]
        public void ParseStatement()
        {
            string raw = File.ReadAllText("files/BANKMSGSRSV1.ofx");
			var parser = new OfxAssimilate.OfxToXmlParser(raw);
			XElement parsed = parser.Parse();
			Assert.AreEqual("+2703.71", parsed.Element("LEDGERBAL").Element("BALAMT").Value);
            
        }

        [Test]
        public void ParseOfx()
        {
            string raw = File.ReadAllText("files/ACCTINFORS.ofx");
			var parser = new OfxAssimilate.OfxToXmlParser(raw);
			XElement parsed = parser.Parse();
			Assert.AreEqual ("CHECKING", parsed.Element ("ACCTINFORS").Element("ACCTINFO").Element ("ACCTTYPE").Value);
			Assert.AreEqual ("MYACCESS CHECKING", parsed.Element ("ACCTINFORS").Element ("ACCTINFO").Element ("DESC").Value);
        }
    }
}
