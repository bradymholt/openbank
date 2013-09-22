using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace OpenBank.OfxAssimilate
{
	public class StatementFetcher : OfxFetcher
	{
		private StatementParameters m_statementParameters;

		public StatementFetcher (StatementParameters parameters)
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
			if (m_statementParameters.AccountType == OfxData.OfxAccountType.CREDITCARD.ToString ()) {
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

		public override OfxResponse BuildResponse (XElement parsedOfx)
		{
			var response = new StatementResponse ();

			var transactions = parsedOfx.Element ("BANKTRANLIST");
			if (transactions != null) {
				response.statement = new OfxData.OfxStatement ();
				response.statement.ledger_balance = new  OfxData.OfxStatementBalance ();
				response.statement.available_balance = new  OfxData.OfxStatementBalance ();

				ExtractBalance (parsedOfx.Element ("LEDGERBAL"), response.statement.ledger_balance);
				ExtractBalance (parsedOfx.Element ("AVAILBAL"), response.statement.available_balance);

				response.statement.transactions =
					(from c in parsedOfx.Descendants ("STMTTRN")
					 let name = ExtractAndScrubElementText (c.Element ("NAME"))
					 let memo = ExtractAndScrubElementText (c.Element ("MEMO"))
					 select new OfxData.OfxStatementTransaction {
					id = c.Element("FITID").Value,
					type = c.Element("TRNTYPE").Value,
					date = ConvertDateTimeToUTC(c.Element("DTPOSTED").Value),
					amount = decimal.Parse(
							c.Element("TRNAMT").Value,
							NumberFormatInfo.InvariantInfo),
					name = name
				}).ToList ();
			}

			return response;
		}

		private void ExtractBalance (XElement ofxSource, OfxData.OfxStatementBalance targetBalance)
		{
			if (ofxSource != null) {
				if (ofxSource.Element ("BALAMT") != null
					&& !string.IsNullOrEmpty (ofxSource.Element ("BALAMT").Value)) {
					targetBalance.amount = Convert.ToDecimal (ofxSource.Element ("BALAMT").Value);
				}

				if (ofxSource.Element ("DTASOF") != null
					&& !string.IsNullOrEmpty (ofxSource.Element ("DTASOF").Value)) {
					targetBalance.date = ConvertDateTimeToUTC (ofxSource.Element ("DTASOF").Value);
				}
			}
		}

		private string ExtractAndScrubElementText (XElement element)
		{
			string result = string.Empty;
			if (element != null && element.Value != null) {
				result = element.Value;
				result = result.Replace ("&amp;#39;", "'");
				result = result.Replace ("&amp;", "&");
			}

			return result;
		}

		private DateTime ConvertDateTimeToUTC (string dateTimeText)
		{
			DateTime baseDateTime = DateTime.ParseExact (dateTimeText.Substring (0, 8), "yyyyMMdd", null);
			DateTime utcDateTime;

			if (dateTimeText.Contains ("[")) {
				// example input: 20120402111107.350[-4:EDT]
				int offSetStart = dateTimeText.IndexOf ("[") + 1;
				int offSetEnd = dateTimeText.IndexOf (":", offSetStart);
				int offSet = Convert.ToInt32 (dateTimeText.Substring (offSetStart, offSetEnd - offSetStart));
				utcDateTime = baseDateTime.AddHours (offSet * -1);  //back out offset to get back to UTC
			} else {
				utcDateTime = baseDateTime;
			}

			return utcDateTime;
		}
	}
}
