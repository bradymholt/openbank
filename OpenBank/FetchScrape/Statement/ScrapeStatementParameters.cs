using System;

namespace OpenBank.FetchScrape
{
	public class ScrapeStatementParameters : ScrapeFetchParameters
	{
		[ParameterRequired]
		public string AccountID { get; set; }

		[ParameterRequired]
		public DateTime DateStart { get; set; }

		[ParameterRequired]
		public DateTime DateEnd { get; set; }
	}
}

