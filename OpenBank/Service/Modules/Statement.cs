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
                    BankCode = request.bank_code,
					AccountType = request.account_type,
					DateStart = ParameterHelper.GetDateParameter(request.date_start),
					DateEnd = ParameterHelper.GetDateParameter(request.date_end)
                };

				var fetcher = new OfxAssimilate.StatementFetcher(requestParameters);
				fetcher.Fetch();

				return Negotiate
					.WithModel(fetcher.Response)
					.WithStatusCode(fetcher.Response.HttpStatus);
            };
        }

        public class Parameters
        {
            public string ofx_url { get; set; }

            public string fid { get; set; }

            public string org { get; set; }

            public string user_id { get; set; }

            public string password { get; set; }

			public string bank_code { get; set; }

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
