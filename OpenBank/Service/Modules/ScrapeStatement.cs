using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace OpenBank.Service.Module
{
	public class ScrapeStatement : NancyModule
	{
		private static NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

		public ScrapeStatement()
		{
			Post["/scrape/statement"] = parameters =>
			{
 				var request = this.Bind<Parameters>();

				var requestParameters = new FetchScrape.ScrapeStatementParameters()
				{
					FID = request.fid,
					UserID = request.user_id,
					Password = request.password,
					SecurityAnswers = request.security_answers,
					AccountID = request.account_id,
					DateStart = ParameterHelper.ParseDateParameter(request.date_start),
					DateEnd = ParameterHelper.ParseDateParameter(request.date_end)
				};

				var fetcher = new FetchScrape.ScrapeStatementFetcher(requestParameters);

				try {
					fetcher.Fetch();
				} catch(Exception ex) {
					s_logger.ErrorException("Error on /scrape/statement", ex);
					throw;
				}

				return Negotiate
				.WithModel(fetcher.Response)
				.WithStatusCode(fetcher.Response.HttpStatus);
			};
		}

		public class Parameters
		{
			public string fid { get; set; }
			public string user_id { get; set; }
			public string password { get; set; }
			public string security_answers {get;set;}

			public string account_id { get; set; }

			[ParameterFormat("YYYYMMDD")]
			public string date_start { get; set; }

			[ParameterFormat("YYYYMMDD")]
			public string date_end { get; set; }
		}
	}
}
