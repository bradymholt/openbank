using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace OpenBank
{
    public class Accounts : NancyModule
    {
        public Accounts()
        {
            Get["/accounts"] = parameters =>
            {
                var request = this.Bind<Parameters>();
                AccountRequestParameters requestParameters = new AccountRequestParameters()
                {
                    URL = request.url,
                    FID = request.fid,
                    ORG = request.org,
                    UserID = request.user_id,
                    Password = request.password
                };

                var fetcher = new AccountRequest(requestParameters);
                OfxResponse ofx = fetcher.FetchOfx();
                return ofx.Account.Accounts;
            };
        }

        public class Parameters
        {
            public string url { get; set; }
            public string fid { get; set; }
            public string org { get; set; }
            public string user_id { get; set; }
            public string password { get; set; }
        }
    }
}