using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace OfxClient
{
    public class OfxTransactions : NancyModule
    {
        public OfxTransactions()
        {
            Post["/transactions"] = parameters =>
            {
                var request = this.Bind<OfxRequest>();
                var fetcher = new DirectFetcher(request);
                OfxAccount foo = fetcher.FetchOfx();
                return Response.AsJson(foo);
            };
        }
    }
}
