using System;
using NUnit.Framework;
using System.Xml.Linq;
using System.IO;
using OpenBank.OfxAssimilate;

namespace OpenBank.Test
{
	[TestFixture()]
	public class AccountsFetcherTest
	{
		[Test()]
		public void TestBuildResponseForChecking ()
		{
			string raw = File.ReadAllText ("files/ACCTINFORS_CHECKING.ofx");
			var parser = new OfxToXmlParser (raw);
			XElement parsed = parser.Parse ();

			var fetcher = new OfxAccountsFetcher (null);
			OfxAccountsResponse response = (OfxAccountsResponse)fetcher.BuildResponse (parsed);

			Assert.AreEqual (1, response.accounts.Count);
			Assert.AreEqual ("MYACCESS CHECKING", response.accounts [0].description);
		}

		[Test()]
		public void TestBuildResponseForCreditCard ()
		{
			string raw = File.ReadAllText ("files/ACCTINFORS_CREDIT_CARD.ofx");
			var parser = new OfxToXmlParser (raw);
			XElement parsed = parser.Parse ();

			var fetcher = new OfxAccountsFetcher (null);
			OfxAccountsResponse response = (OfxAccountsResponse)fetcher.BuildResponse (parsed);

			Assert.AreEqual (2, response.accounts.Count);
			Assert.AreEqual ("CREDIT CARD", response.accounts [0].description);
		}
	}
}

