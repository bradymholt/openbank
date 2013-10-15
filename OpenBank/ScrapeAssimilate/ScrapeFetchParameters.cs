using System;

namespace OpenBank.ScrapeAssimilate
{
	public class ScrapeFetchParameters
	{
		[OfxAssimilate.ParameterRequired]
		public string FID {get;set;}

		[OfxAssimilate.ParameterRequired]
		public string UserID { get; set; }

		[OfxAssimilate.ParameterRequired]
		public string Password { get; set; }

		public string SecurityAnswers {get;set;}
	}
}

