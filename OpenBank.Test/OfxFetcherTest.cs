using NUnit.Framework;
using System;

namespace OpenBank.Test
{
	[TestFixture()]
	public class OfxFetcherTest
	{
		[Test()]
		public void TestGenerateRandomString ()
		{
			Assert.AreEqual (10, OfxAssimilate.OfxFetcher.GenerateRandomString (10).Length);
			Assert.AreEqual ("XXXXX", OfxAssimilate.OfxFetcher.GenerateRandomString (new char[] { 'X' }, 5));
		}
	}
}

