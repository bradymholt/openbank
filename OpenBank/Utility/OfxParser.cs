using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace OpenBank
{
    //Adapted from: http://xamlcoder.com/blog/2010/10/29/parsing-ofx-files-in-silverlight/

    public class OfxParser
    {
        public OfxAccount LoadAccount(string ofxData)
        {
            XElement parsedOfx = Parse(ofxData);

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

            OfxAccount account = new OfxAccount
            {
                Organization = organization != null ? organization.Value : null,
                AccountID = accountID != null ? accountID.Value : null,
                AccountType = accountType != null ? accountType.Value : null,
                BankID = bankID != null ? bankID.Value : null,
                LedgerBalanceAmount = ledgerBalanceAmount,
                LedgerBalanceAsOfDate = ledgerBalanceDate,
                AvailableBalanceAmount = availBalanceAmount,
                AvailableBalanceAsOfDate = availBalanceDate
            };

            account.Transactions =
                (from c in parsedOfx.Descendants("STMTTRN")
                 let name = ExtractAndScrubElementText(c.Element("NAME"))
                 let memo = ExtractAndScrubElementText(c.Element("MEMO"))
                 select new OfxTransaction
                 {
                     TransactionID = c.Element("FITID").Value,
                     TransactionType = c.Element("TRNTYPE").Value,
                     Date = ConvertDateTimeToUTC(c.Element("DTPOSTED").Value),
                     Amount = decimal.Parse(
                                     c.Element("TRNAMT").Value,
                                     NumberFormatInfo.InvariantInfo),
                     Name = name,
                     Memo = memo
                 });

            return account;
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


        /// <summary>
        /// Parses the ofx file data and turns it into valid xml.
        /// </summary>
        /// <param name="ofxData">The ofx data.</param>
        /// <returns>An xml representation of the ofx data.</returns>
        public XElement Parse(string ofxData)
        {
            // add additional returns to allow for parsing
            // ofx files that are all on one line
            string data = ofxData.Replace("<", "\n\r<");

            string[] lines = data.Split(
                    new string[] { "\n", "\r" },
                    StringSplitOptions.RemoveEmptyEntries);

            // use linq to get the organization and account tags
            var orgAndAccountTags = from line in lines
                                    where line.Contains("<ORG>")
                                    || line.Contains("BANKID")
                                    || line.Contains("<ACCTID>")
                                    || line.Contains("<ACCTTYPE>")
                                    select line;

            var balanceTags = from line in lines
                              where line.Contains("<LEDGERBAL")
                              || line.Contains("<AVAILBAL>")
                              || line.Contains("<DTASOF>")
                              || line.Contains("<BALAMT>")
                              select line;

            // use linq to get the transaction tags
            var transactionTags = from line in lines
                                  where line.Contains("<STMTTRN>")
                                  || line.Contains("<TRNTYPE>")
                                  || line.Contains("<DTPOSTED>")
                                  || line.Contains("<TRNAMT>")
                                  || line.Contains("<FITID>")
                                  || line.Contains("<CHECKNUM>")
                                  || line.Contains("<NAME>")
                                  || line.Contains("<MEMO>")
                                  select line;

            XElement root = new XElement("OFX");
            XElement child = null;

            // parse organization and account data
            foreach (var line in orgAndAccountTags)
            {
                var tagName = GetTagName(line);
                var elementChild = new XElement(tagName)
                {
                    Value = GetTagValue(line)
                };

                root.Add(elementChild);
            }

            //parse account balances
            foreach (var line in balanceTags)
            {
                if (line.Contains("<LEDGERBAL") || line.Contains("<AVAILBAL"))
                {
                    child = new XElement(GetTagName(line));
                    root.Add(child);
                }
                else
                {
                    var tagName = GetTagName(line);
                    var elementChild = new XElement(tagName)
                    {
                        Value = GetTagValue(line)
                    };

                    child.Add(elementChild);
                }
            }

            // parse transactions
            XElement transactions = new XElement("BANKTRANLIST");
            root.Add(transactions);

            foreach (var line in transactionTags)
            {
                if (line.IndexOf("<STMTTRN>") != -1)
                {
                    child = new XElement("STMTTRN");
                    transactions.Add(child);
                    continue;
                }

                var tagName = GetTagName(line);
                var elementChild = new XElement(tagName)
                {
                    Value = GetTagValue(line)
                };

                child.Add(elementChild);
            }

            return root;
        }

        /// <summary>
        /// Get the Tag name to create an Xelement
        /// </summary>
        /// <param name="line">One line from the file</param>
        /// <returns></returns>
        private static string GetTagName(string line)
        {
            int pos_init = line.IndexOf("<") + 1;
            int pos_end = line.IndexOf(">");
            pos_end = pos_end - pos_init;
            return line.Substring(pos_init, pos_end);
        }

        /// <summary>
        /// Get the value of the tag to put on the Xelement
        /// </summary>
        /// <param name="line">The line</param>
        /// <returns></returns>
        private static string GetTagValue(string line)
        {
            int pos_init = line.IndexOf(">") + 1;
            string retValue = line.Substring(pos_init).Trim();

            // the date contains a time zone offset
            if (retValue.IndexOf("[") != -1)
            {
                // Date - get only 8 date digits
                retValue = retValue.Substring(0, 8);
            }

            // if the value is exactly 18 digits and the 14th digit is a dot
            // this should be a date - trim it
            if (retValue.Length == 18 && retValue.IndexOf(".") == 14)
            {
                // Date - get only 8 date digits
                retValue = retValue.Substring(0, 8);
            }

            return retValue;
        }
    }
}
