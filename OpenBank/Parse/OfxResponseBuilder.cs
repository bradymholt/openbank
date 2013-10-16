using System;
using System.Linq;
using System.Xml.Linq;
using System.Globalization;

namespace OpenBank.Parse
{
	public class OfxResponseBuilder
	{
		public OfxResponseBuilder ()
		{
		}

		public DTO.AccountsResponse BuildAccountsResponse(XElement parsedOfx){
			var response = new DTO.AccountsResponse ();

			var accounts = parsedOfx.Element ("ACCTINFORS");
			if (accounts != null) {
				response.accounts =
					(from c in accounts.Descendants("ACCTINFO")
					 select new DTO.Account {
						bank_id = ((c.Element("BANKID") != null) ? c.Element("BANKID").Value : ""),
						account_id = c.Element("ACCTID").Value,
						account_type = c.Element("ACCTTYPE").Value,
						description = c.Element("DESC").Value
					}).ToList ();
			}
			else {
				throw new OfxParseException ("ACCTINFORS element missing.");
			}

			return response;
		}

		public DTO.StatementResponse BuildStatementResponse(XElement parsedOfx, DateTime startDate, DateTime endDate){
			var response = new DTO.StatementResponse ();

			var transactions = parsedOfx.Element ("BANKTRANLIST");
			if (transactions != null) {
				response.statement = new DTO.Statement ();
				response.statement.ledger_balance = new  DTO.StatementBalance ();
				response.statement.available_balance = new  DTO.StatementBalance ();

				ExtractBalance (parsedOfx.Element ("LEDGERBAL"), response.statement.ledger_balance);
				ExtractBalance (parsedOfx.Element ("AVAILBAL"), response.statement.available_balance);

				var ofxTransactions =
					(from c in parsedOfx.Descendants ("STMTTRN")
					 let name = ExtractAndScrubElementText (c.Element ("NAME"))
					 let memo = ExtractAndScrubElementText (c.Element ("MEMO"))
					 select new DTO.StatementTransaction {
						id = c.Element ("FITID").Value,
						type = c.Element ("TRNTYPE").Value,
						date = ConvertDateTimeToUTC (c.Element ("DTPOSTED").Value),
						amount = decimal.Parse (
							c.Element ("TRNAMT").Value,
							NumberFormatInfo.InvariantInfo),
						name = name
					}).ToList ();

				response.statement.transactions = ofxTransactions
					.Where (t => t.date >= startDate && t.date <= endDate)
						.ToList ();
			}

			return response;
		}

		private void ExtractBalance (XElement ofxSource, DTO.StatementBalance targetBalance)
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

