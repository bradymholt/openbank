using System;

namespace OpenBank.ScrapeAssimilate
{
	public class ScrapeStatementParameters : ScrapeFetchParameters
	{
		[OfxAssimilate.ParameterRequired]
		public string AccountID { get; set; }

		[OfxAssimilate.ParameterRequired]
		public DateTime DateStart { get; set; }

		[OfxAssimilate.ParameterRequired]
		public DateTime DateEnd { get; set; }
	}
}

