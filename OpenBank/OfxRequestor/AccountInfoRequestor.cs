using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class AccountInfoRequestor : OfxRequestor
    {
        AccountInfoRequestParameters m_parameters;

        public AccountInfoRequestor(AccountInfoRequestParameters parameters)
            :base(parameters)
        {
            m_parameters = parameters;
        }

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

        protected override string BuildRequestInnerBody()
        {
            string request = string.Format(OFX_ACCOUNT_LIST_REQUEST, GenerateRandomString(8));
            return request;
        }
    }
}
