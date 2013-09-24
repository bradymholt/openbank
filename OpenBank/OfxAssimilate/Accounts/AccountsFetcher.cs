	using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpenBank.OfxAssimilate
{
    public class AccountsFetcher : OfxFetcher
    {
		public AccountsFetcher(AccountsParameters parameters)
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

		public override OfxResponse BuildResponse(XElement parsedOfx){
			var response = new AccountsResponse ();

			var accounts = parsedOfx.Element ("ACCTINFORS");
			if (accounts != null) {
				response.accounts =
					(from c in accounts.Descendants("ACCTINFO")
					 select new OfxData.OfxAccount {
						bank_code = ((c.Element("BANKID") != null) ? c.Element("BANKID").Value : ""),
						account_id = c.Element("ACCTID").Value,
						account_type = c.Element("ACCTTYPE").Value,
						description = c.Element("DESC").Value
					}).ToList ();
			}

			return response;
		}
    }
}
