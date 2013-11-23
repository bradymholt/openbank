using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace OpenBank.Service.Module
{
	public class Accounts : NancyModule
	{
		private static NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

		public Accounts ()
		{
			//wget --header="Accept: application/json" --post-data="ofx_url=https://ofx.bankofamerica.com/cgi-forte/fortecgi?servicename=ofx_2-3%26pagename=ofx&fid=6812&org=HAN&user_id=user&password=pass" http://localhost:1234/accounts
			Post ["/ofx/accounts"] = parameters =>
			{
				var request = this.Bind<Parameters> ();
				var requestParameters = new FetchOfx.OfxAccountsParameters () {
					OFX_URL = request.ofx_url,
					FID = request.fid,
					ORG = request.org,
					UserID = request.user_id,
					Password = request.password
				};

				var fetcher = new FetchOfx.OfxAccountsFetcher (requestParameters);

				try {
					fetcher.Fetch();
				} catch(Exception ex) {
					s_logger.ErrorException("Error on /ofx/accounts", ex);
					throw;
				}

				return Negotiate
					.WithModel(fetcher.Response)
					.WithStatusCode(fetcher.Response.HttpStatus);
			};
		}

		public class Parameters
		{
			[ParameterFormat("http://bank.com/service-path")]
			public string ofx_url { get; set; }
			public string fid { get; set; }
			public string org { get; set; }
			public string user_id { get; set; }
			public string password { get; set; }
		}
	}
}