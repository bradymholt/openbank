using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class StatementRequestor : OfxRequestor
    {
        private StatementParameters m_parameters;
        public StatementRequestor(StatementParameters parameters)
            :base (parameters)
        {
            m_parameters = parameters;
        }

        private const string BANKING_STATEMENT_REQUEST =
             " <BANKMSGSRQV1>\n"
           + "  <STMTTRNRQ>\n"
           + "   <TRNUID>{0}\n"
           + "   <CLTCOOKIE>{1}\n"
           + "   <STMTRQ>\n"
           + "    <BANKACCTFROM>\n"
           + "     <BANKID>{2}\n"
           + "     <ACCTID>{3}\n"
           + "     <ACCTTYPE>{4}\n"
           + "    </BANKACCTFROM>\n"
           + "    <INCTRAN>\n"
           + "     <DTSTART>{5}\n"
           + "     <INCLUDE>Y\n"
           + "    </INCTRAN>\n"
           + "   </STMTRQ>\n"
           + "  </STMTTRNRQ>\n"
           + " </BANKMSGSRQV1>";

        private const string CREDIT_CARD_STATEMENT_REQUEST =
              " <CREDITCARDMSGSRQV1>\n"
            + "  <CCSTMTTRNRQ>\n"
            + "   <TRNUID>{0}\n"
            + "   <CLTCOOKIE>{1}\n"
            + "   <CCSTMTRQ>\n"
            + "    <CCACCTFROM>\n"
            + "     <ACCTID>{2}\n"
            + "    </CCACCTFROM>\n"
            + "    <INCTRAN>\n"
            + "     <DTSTART>{3}\n"
            + "     <INCLUDE>Y\n"
            + "    </INCTRAN>\n"
            + "   </CCSTMTRQ>\n"
            + "  </CCSTMTTRNRQ>\n"
            + " </CREDITCARDMSGSRQV1>";

        protected override string BuildRequestInnerBody()
        {
            string request = string.Empty;
            switch (m_parameters.AccountType)
            {
                case AccountType.CREDITCARD:
                    request = string.Format(CREDIT_CARD_STATEMENT_REQUEST,
                        GenerateRandomString(8),
                        GenerateRandomString(5),
                        m_parameters.AccountID,
                        m_parameters.DateFrom.ToString("yyyyMMddHHmmss.fff")); //UTC
                    break;
                default:
                    request = string.Format(BANKING_STATEMENT_REQUEST,
                        GenerateRandomString(8),
                        GenerateRandomString(5),
                        m_parameters.BankID,
                        m_parameters.AccountID,
                        m_parameters.AccountType.ToString(),
                        m_parameters.DateFrom.ToString("yyyyMMddHHmmss.fff")); //UTC
                    break;

            }

            return request;
        }
    }
}
