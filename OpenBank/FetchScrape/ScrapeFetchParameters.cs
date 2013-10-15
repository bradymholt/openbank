using System;

namespace OpenBank.FetchScrape
{
	public class ScrapeFetchParameters
	{
		[ParameterRequired]
		public string FID {get;set;}

		[ParameterRequired]
		public string UserID { get; set; }

		[ParameterRequired]
		public string Password { get; set; }

		public string SecurityAnswers {get;set;}
	}
}

