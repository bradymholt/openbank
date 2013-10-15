	using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenBank.FetchOfx
{
    public class OfxAccountsFetcher : OfxFetcher
    {
		public OfxAccountsFetcher(OfxAccountsParameters parameters)
		:base(parameters) { }

        private const string OFX_ACCOUNT_LIST_REQUEST =
            " <SIGNUPMSGSRQV1>\n"
          + "  <ACCTINFOTRNRQ>\n"
          + "   <TRNUID>{0}\n"
          + "   <CLTCOOKIE>4\n"
          + "   <ACCTINFORQ>\n"
          + "    <DTACCTUP>19700101000000\n"
          + "   </ACCTINFORQ>\n"
          + "  </ACCTINFOTRNRQ>\n"
          + " </SIGNUPMSGSRQV1>";

		public override string BuildRequestInnerBody()
        {
            string request = string.Format(OFX_ACCOUNT_LIST_REQUEST, GenerateRandomString(8));
            return request;
        }

		public override DTO.ServiceResponse BuildResponse(XElement parsedOfx)
		{
			Parse.OfxResponseBuilder responseBuilder = new Parse.OfxResponseBuilder();
			var response = responseBuilder.BuildAccountsResponse (parsedOfx);
			return response;
		}
    }
}
