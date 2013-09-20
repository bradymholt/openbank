//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Xml.Linq;
//
//namespace OpenBank
//{
//	public class OfxResponseBuilder
//	{
//		public OfxResponse Build(string rawOfx)
//		{
//			OfxToXmlParser parser = new OfxToXmlParser ();
//			XElement parsedOfx = parser.ParseRaw(rawOfx);
//
//			OfxResponseBuilder builder = new OfxResponseBuilder ();
//			OfxResponse ofxResponse = builder.Build (parsedOfx);
//
//			return ofxResponse;
//		}
//
//		public OfxResponse Build(XElement parsedOfx)
//		{
//			OfxResponse ofx = new OfxResponse ();
//
//			var status = parsedOfx.Element ("STATUS");
//			if (status != null) {
//				ofx.status = new Status ();
//				try {
//					ofx.status.code = status.Element ("CODE").Value;
//					ofx.status.severity = status.Element ("SEVERITY").Value;
//					if (status.Element ("MESSAGE") != null) {
//						ofx.status.message = status.Element ("MESSAGE").Value;
//					}
//				} catch (Exception ex) {
//					ofx.status.message = "Error when parsing status: " + ex.Message;
//				}
//			}
//
//			var accounts = parsedOfx.Element ("ACCTINFORS");
//			if (accounts != null) {
//				ofx.accounts =
//                (from c in parsedOfx.Descendants ("ACCTINFO")
//                 select new AccountInfo {
//					bank_id = c.Element("BANKID").Value,
//					account_id = c.Element("ACCTID").Value,
//					account_type = c.Element("ACCTTYPE").Value,
//					description = c.Element("DESC").Value
//				}).ToList ();
//			}
//
//			var transactions = parsedOfx.Element ("BANKTRANLIST");
//			if (transactions != null) {
//				ofx.statement = new StatementResponse ();
//
//				var ledgerBalance = parsedOfx.Element ("LEDGERBAL");
//				var availBalance = parsedOfx.Element ("AVAILBAL");
//
//				decimal? ledgerBalanceAmount = null;
//				DateTime? ledgerBalanceDate = null;
//				if (ledgerBalance != null) {
//					if (ledgerBalance.Element ("BALAMT") != null
//						&& !string.IsNullOrEmpty (ledgerBalance.Element ("BALAMT").Value)) {
//						ledgerBalanceAmount = Convert.ToDecimal (ledgerBalance.Element ("BALAMT").Value);
//					}
//
//					if (ledgerBalance.Element ("DTASOF") != null
//						&& !string.IsNullOrEmpty (ledgerBalance.Element ("DTASOF").Value)) {
//						ledgerBalanceDate = ConvertDateTimeToUTC (ledgerBalance.Element ("DTASOF").Value);
//					}
//				}
//
//				decimal? availBalanceAmount = null;
//				DateTime? availBalanceDate = null;
//				if (availBalance != null) {
//					if (availBalance.Element ("BALAMT") != null
//						&& !string.IsNullOrEmpty (availBalance.Element ("BALAMT").Value)) {
//						availBalanceAmount = Convert.ToDecimal (availBalance.Element ("BALAMT").Value);
//					}
//
//					if (availBalance.Element ("DTASOF") != null
//						&& !string.IsNullOrEmpty (availBalance.Element ("DTASOF").Value)) {
//						availBalanceDate = ConvertDateTimeToUTC (ledgerBalance.Element ("DTASOF").Value);
//					}
//				}
//
//				ofx.statement.ledger_balance = new StatementBalance () {
//					amount = ledgerBalanceAmount.Value,
//					date = ledgerBalanceDate.Value
//				};
//				ofx.statement.available_balance = new StatementBalance () {
//					amount = availBalanceAmount.Value,
//					date = availBalanceDate.Value
//				};
//
//				ofx.statement.transactions =
//                    (from c in parsedOfx.Descendants ("STMTTRN")
//                     let name = ExtractAndScrubElementText (c.Element ("NAME"))
//                     let memo = ExtractAndScrubElementText (c.Element ("MEMO"))
//                     select new StatementTransaction {
//					id = c.Element("FITID").Value,
//					type = c.Element("TRNTYPE").Value,
//					date = ConvertDateTimeToUTC(c.Element("DTPOSTED").Value),
//					amount = decimal.Parse(
//                                         c.Element("TRNAMT").Value,
//                                         NumberFormatInfo.InvariantInfo),
//					name = name
//				}).ToList ();
//			}
//
//			return ofx;
//		}
//
//
//	}
//}
