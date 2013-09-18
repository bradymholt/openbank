using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace OpenBank
{
    public class StatementService : NancyModule
    {
        public StatementService()
        {
            Post["/transactions"] = parameters =>
            {
                var request = this.Bind<Parameters>();

                StatementParameters requestParameters = new StatementParameters()
                {
                    URL = request.url,
                    FID = request.fid,
                    ORG = request.org,
                    UserID = request.user_id,
                    Password = request.password,
                    AccountID = request.account_id,
                    BankID = request.bank_id,
                    AccountType = request.account_type,
                    DateFrom = ServiceParameterParsingHelper.GetDateParameter(request.date_from)
                };

                var fetcher = new StatementRequestor(requestParameters);
                OfxResponse ofx = fetcher.FetchOfx();

                if (!ofx.is_error)
                {
                    return ofx.statement;
                }
                else
                {
                    return ofx.ofx_error_message;
                }
            };
        }

        public class Parameters
        {
            public string url { get; set; }
            public string fid { get; set; }
            public string org { get; set; }
            public string user_id { get; set; }
            public string password { get; set; }

            public string bank_id { get; set; }
            public string account_id { get; set; }

            [AccountTypeFormat]
            public AccountType account_type { get; set; }

            [ParameterFormat("YYYYMMDD")]
            public string date_from { get; set; }
        }
    }
}
