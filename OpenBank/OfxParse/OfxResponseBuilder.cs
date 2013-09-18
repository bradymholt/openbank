using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OpenBank
{
    public class OfxResponseBuilder
    {
        public OfxResponse Build(XElement parsedOfx)
        {
            OfxResponse ofx = new OfxResponse();

            var accounts = parsedOfx.Element("ACCTINFORS");
            if (accounts != null)
            {
                ofx.accounts =
                (from c in parsedOfx.Descendants("ACCTINFO")
                 select new AccountInfo
                 {
                     bank_id = c.Element("BANKID").Value,
                     account_id = c.Element("ACCTID").Value,
                     account_type = c.Element("ACCTTYPE").Value,
                     description = c.Element("DESC").Value
                 }).ToList();
            }

            var transactions = parsedOfx.Element("BANKTRANLIST");
            if (transactions != null)
            {
                ofx.statement = new StatementResponse();

                var accountID = parsedOfx.Element("ACCTID");
                var accountType = parsedOfx.Element("ACCTTYPE");
                var bankID = parsedOfx.Element("BANKID");
                var organization = parsedOfx.Element("ORG");
                var ledgerBalance = parsedOfx.Element("LEDGERBAL");
                var availBalance = parsedOfx.Element("AVAILBAL");

                decimal? ledgerBalanceAmount = null;
                DateTime? ledgerBalanceDate = null;
                if (ledgerBalance != null)
                {
                    if (ledgerBalance.Element("BALAMT") != null
                        && !string.IsNullOrEmpty(ledgerBalance.Element("BALAMT").Value))
                    {
                        ledgerBalanceAmount = Convert.ToDecimal(ledgerBalance.Element("BALAMT").Value);
                    }

                    if (ledgerBalance.Element("DTASOF") != null
                        && !string.IsNullOrEmpty(ledgerBalance.Element("DTASOF").Value))
                    {
                        ledgerBalanceDate = ConvertDateTimeToUTC(ledgerBalance.Element("DTASOF").Value);
                    }
                }

                decimal? availBalanceAmount = null;
                DateTime? availBalanceDate = null;
                if (availBalance != null)
                {
                    if (availBalance.Element("BALAMT") != null
                        && !string.IsNullOrEmpty(availBalance.Element("BALAMT").Value))
                    {
                        availBalanceAmount = Convert.ToDecimal(availBalance.Element("BALAMT").Value);
                    }

                    if (availBalance.Element("DTASOF") != null
                       && !string.IsNullOrEmpty(availBalance.Element("DTASOF").Value))
                    {
                        availBalanceDate = ConvertDateTimeToUTC(ledgerBalance.Element("DTASOF").Value);
                    }
                }

                ofx.statement.ledger_balance = new StatementBalance() { amount = ledgerBalanceAmount.Value, date = ledgerBalanceDate.Value };
                ofx.statement.available_balance = new StatementBalance() { amount = availBalanceAmount.Value, date = availBalanceDate.Value };

                ofx.statement.transactions =
                    (from c in parsedOfx.Descendants("STMTTRN")
                     let name = ExtractAndScrubElementText(c.Element("NAME"))
                     let memo = ExtractAndScrubElementText(c.Element("MEMO"))
                     select new StatementTransaction
                     {
                         id = c.Element("FITID").Value,
                         type = c.Element("TRNTYPE").Value,
                         date = ConvertDateTimeToUTC(c.Element("DTPOSTED").Value),
                         amount = decimal.Parse(
                                         c.Element("TRNAMT").Value,
                                         NumberFormatInfo.InvariantInfo),
                         name = name
                     }).ToList();
            }

            return ofx;
        }

        private string ExtractAndScrubElementText(XElement element)
        {
            string result = string.Empty;
            if (element != null && element.Value != null)
            {
                result = element.Value;
                result = result.Replace("&amp;#39;", "'");
                result = result.Replace("&amp;", "&");
            }

            return result;
        }

        private DateTime ConvertDateTimeToUTC(string dateTimeText)
        {
            DateTime baseDateTime = DateTime.ParseExact(dateTimeText.Substring(0, 8), "yyyyMMdd", null);
            DateTime utcDateTime;

            if (dateTimeText.Contains("["))
            {
                // example input: 20120402111107.350[-4:EDT]
                int offSetStart = dateTimeText.IndexOf("[") + 1;
                int offSetEnd = dateTimeText.IndexOf(":", offSetStart);
                int offSet = Convert.ToInt32(dateTimeText.Substring(offSetStart, offSetEnd - offSetStart));
                utcDateTime = baseDateTime.AddHours(offSet * -1);  //back out offset to get back to UTC
            }
            else
            {
                utcDateTime = baseDateTime;
            }

            return utcDateTime;
        }
    }
}
