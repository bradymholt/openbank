using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenBank.Test
{
	[TestFixture()]
	public class TestScrapeAccountsJson
	{
		[Test()]
		public void TestCase ()
		{
			string accountsJson = File.ReadAllText ("files/accounts.json");
			List<OfxData.OfxAccount> accounts = JsonConvert.DeserializeObject<List<OfxData.OfxAccount>>(accountsJson);
			Assert.AreEqual (accounts [0].description, "CHECKING");
		}
	}
}

