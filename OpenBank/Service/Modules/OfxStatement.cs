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
		private static NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public Statement()
        {
			Post["/ofx/statement"] = parameters =>
            {
                var request = this.Bind<Parameters>();

                var requestParameters = new FetchOfx.OfxStatementParameters()
                {
                    OFX_URL = request.ofx_url,
                    FID = request.fid,
                    ORG = request.org,
                    UserID = request.user_id,
                    Password = request.password,
                    AccountID = request.account_id,
                    BankID = request.bank_id,
					AccountType = request.account_type,
					DateStart = ParameterHelper.ParseDateParameter(request.date_start),
					DateEnd = ParameterHelper.ParseDateParameter(request.date_end)
                };

				var fetcher = new FetchOfx.OfxStatementFetcher(requestParameters);

				try {
					fetcher.Fetch();
				} catch(Exception ex) {
					s_logger.ErrorException("Error on /ofx/statements", ex);
					throw;
				}

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
