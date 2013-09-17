using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace OpenBank
{
    public class Transactions : NancyModule
    {
        public Transactions()
        {
            Post["/transactions"] = parameters =>
            {
                var request = this.Bind<Parameters>();

                StatementRequestParameters requestParameters = new StatementRequestParameters()
                {
                    URL = request.url,
                    FID = request.fid,
                    ORG = request.org,
                    UserID = request.user_id,
                    Password = request.password,
                    AccountID = request.account_id,
                    BankID = request.bank_id,
                    AccountType = request.type,
                    DateStart = request.date_start,
                    DateEnd = request.date_end
                };

                var fetcher = new StatementRequest(requestParameters);
                OfxResponse ofx = fetcher.FetchOfx();
                return ofx.Statement;
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
            public AccountType type { get; set; }

            public DateTime date_start { get; set; }
            public DateTime date_end { get; set; }

        }
    }
}
