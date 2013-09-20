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
    public class Statement : NancyModule
    {
        public Statement()
        {
			Post["/statement"] = parameters =>
            {
                var request = this.Bind<Parameters>();

                var requestParameters = new OfxAssimilate.StatementParameters()
                {
                    OFX_URL = request.ofx_url,
                    FID = request.fid,
                    ORG = request.org,
                    UserID = request.user_id,
                    Password = request.password,
                    AccountID = request.account_id,
                    BankID = request.bank_id,
					AccountType = (OfxData.OfxAccountType)Enum.Parse(typeof(OfxData.OfxAccountType), request.account_type),
					DateStart = ParameterHelper.GetDateParameter(request.date_start),
					DateEnd = ParameterHelper.GetDateParameter(request.date_end)
                };

				var fetcher = new OfxAssimilate.StatementFetcher(requestParameters);

				if (fetcher.Fetch ()){
					return fetcher.Response;
				}
				else{
					return Response.AsJson (fetcher.Response, (HttpStatusCode)((OfxAssimilate.OfxResponseError)fetcher.Response).http_status_code);
				}
            };
        }

        public class Parameters
        {
            public string ofx_url { get; set; }
            public string fid { get; set; }
            public string org { get; set; }
            public string user_id { get; set; }
            public string password { get; set; }

            public string bank_id { get; set; }
            public string account_id { get; set; }

            [AccountTypeValues]
            public string account_type { get; set; }

            [ParameterFormat("YYYYMMDD")]
            public string date_start { get; set; }

			[ParameterFormat("YYYYMMDD")]
			public string date_end { get; set; }
        }
    }
}
