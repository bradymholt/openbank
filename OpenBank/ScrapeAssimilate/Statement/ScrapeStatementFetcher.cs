using System;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using NLog;


namespace OpenBank.ScrapeAssimilate
{
	public class ScrapeStatementFetcher : ScrapeFetcher
	{
		private ScrapeStatementParameters m_statementParams;
		public ScrapeStatementFetcher (ScrapeStatementParameters parameters)
			:base(parameters)
		{
			m_statementParams = parameters;
		}

		protected override string GetScriptName (string fid)
		{
			return string.Format("{0}_statement.js", fid);
		}

		protected override void PrepScrape (System.Diagnostics.Process process)
		{
			process.StartInfo.Arguments += " --account_id=" + m_statementParams.AccountID;
			process.StartInfo.Arguments += " --from_date=" + m_statementParams.DateStart.ToShortDateString();
			process.StartInfo.Arguments += " --to_date=" + m_statementParams.DateEnd.ToShortDateString();
		}

		protected override OpenBank.OfxAssimilate.OfxResponse ProcessScrape (string outputPath)
		{
			OfxAssimilate.OfxResponse response = null;
			string statementFilePath = Path.Combine (outputPath, "statement.qfx");
			if (File.Exists(statementFilePath)) {
				string statementOfx = File.ReadAllText(statementFilePath);
				OfxAssimilate.OfxToXmlParser parser = new OfxAssimilate.OfxToXmlParser (statementOfx);
				XElement parsedOfx = parser.Parse ();

				var r = new OfxAssimilate.OfxStatementResponse ();

				var transactions = parsedOfx.Element ("BANKTRANLIST");
				if (transactions != null) {
					r.statement = new OfxData.OfxStatement ();
					r.statement.ledger_balance = new  OfxData.OfxStatementBalance ();
					r.statement.available_balance = new  OfxData.OfxStatementBalance ();

					ExtractBalance (parsedOfx.Element ("LEDGERBAL"), r.statement.ledger_balance);
					ExtractBalance (parsedOfx.Element ("AVAILBAL"), r.statement.available_balance);

					var ofxTransactions  =
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

					r.statement.transactions = ofxTransactions
						.Where (t => t.date >= m_statementParams.DateStart && t.date <= m_statementParams.DateEnd)
							.ToList ();

					response = r;
				}

			
			} else {
				response = new OfxAssimilate.OfxResponseError (HttpStatusCode.BadRequest) {
					friendly_error = "An error occured when atempting to get account statement.",
					detailed_error = "statement file missing" 
				};
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

