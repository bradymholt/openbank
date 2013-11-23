using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace OpenBank.Service.Module
{
	public class ScrapeAccounts : NancyModule
	{
		private static NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

		public ScrapeAccounts ()
		{
			//wget --header="Accept: application/json" --post-data="ofx_url=https://ofx.bankofamerica.com/cgi-forte/fortecgi?servicename=ofx_2-3%26pagename=ofx&fid=6812&org=HAN&user_id=user&password=pass" http://localhost:1234/accounts
			Post["/scrape/accounts"] = parameters =>
			{
				var request = this.Bind<Parameters> ();
				var requestParameters = new FetchScrape.ScrapeAccountsParameters () {
					FID = request.fid,
					UserID = request.user_id,
					Password = request.password,
					SecurityAnswers = request.security_answers
				};

				var fetcher = new FetchScrape.ScrapeAccountsFetcher(requestParameters);

				try {
					fetcher.Fetch();
				} catch(Exception ex) {
					s_logger.ErrorException("Error on /scrape/accounts", ex);
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
		}
	}
}