using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace OpenBank.FetchOfx
{
	public class OfxStatementFetcher : OfxFetcher
	{
		private OfxStatementParameters m_statementParameters;

		public OfxStatementFetcher (OfxStatementParameters parameters)
            :base (parameters)
		{
			m_statementParameters = parameters;
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
			+ "     <DTEND>{6}\n"
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
			+ "     <DTEND>{4}\n"
			+ "     <INCLUDE>Y\n"
			+ "    </INCTRAN>\n"
			+ "   </CCSTMTRQ>\n"
			+ "  </CCSTMTTRNRQ>\n"
			+ " </CREDITCARDMSGSRQV1>";

		public override string BuildRequestInnerBody ()
		{
			string request = string.Empty;
			if (m_statementParameters.AccountType == DTO.AccountType.CREDITCARD.ToString ()) {
				request = string.Format (CREDIT_CARD_STATEMENT_REQUEST,
                     GenerateRandomString (8),
                     GenerateRandomString (5),
                     m_statementParameters.AccountID,
                     m_statementParameters.DateStart.ToString ("yyyyMMdd"),
                     m_statementParameters.DateEnd.ToString ("yyyyMMdd"));
			} else {
				request = string.Format (BANKING_STATEMENT_REQUEST,
	                 GenerateRandomString (8),
	                 GenerateRandomString (5),
	                 m_statementParameters.BankID,
	                 m_statementParameters.AccountID,
	                 m_statementParameters.AccountType,
	                 m_statementParameters.DateStart.ToString ("yyyyMMdd"),
	                 m_statementParameters.DateEnd.ToString ("yyyyMMdd"));
			}

			return request;
		}

		public override DTO.ServiceResponse BuildResponse (XElement parsedOfx)
		{
			Parse.OfxResponseBuilder responseBuilder = new Parse.OfxResponseBuilder();
			var response = responseBuilder.BuildStatementResponse (parsedOfx, m_statementParameters.DateStart, m_statementParameters.DateEnd);
			return response;
		}


	}
}
