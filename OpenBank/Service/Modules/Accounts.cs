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
		public Accounts ()
		{
			Post ["/accounts"] = parameters =>
			{
				var request = this.Bind<Parameters> ();
				var requestParameters = new OfxAssimilate.AccountsParameters () {
					OFX_URL = request.ofx_url,
					FID = request.fid,
					ORG = request.org,
					UserID = request.user_id,
					Password = request.password
				};

				var fetcher = new OfxAssimilate.AccountsFetcher (requestParameters);

				if (fetcher.Fetch ()){
					return fetcher.Response;
				}
				else{
					return Response.AsJson (fetcher.Response, (HttpStatusCode)fetcher.Response.HttpStatus);
				}
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