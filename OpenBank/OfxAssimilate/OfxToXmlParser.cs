using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace OpenBank.OfxAssimilate
{
	//Adapted from: http://xamlcoder.com/blog/2010/10/29/parsing-ofx-files-in-silverlight/
	public class OfxToXmlParser
	{
		private const string STATUS_CODE_SUCCESS = "0";

		private string m_rawOfx;

		public OfxToXmlParser (string rawOfx)
		{
			m_rawOfx = rawOfx;
		}

		/// <summary>
		/// Parses the ofx file data and turns it into valid xml.
		/// </summary>
		/// <param name="rawOfx">The ofx data.</param>
		/// <returns>An xml representation of the ofx data.</returns>
		public XElement Parse ()
		{
			XElement root = new XElement ("OFX");

			try {
				if (string.IsNullOrEmpty(m_rawOfx)){
					throw new OfxParseException("rawOfx is empty.");
				}

				// add additional returns to allow for parsing
				// ofx files that are all on one line
				string data = m_rawOfx.Replace ("<", "\n\r<");

				string[] lines = data.Split (
					new string[] { "\n", "\r" },
					StringSplitOptions.RemoveEmptyEntries);

				List<string> statusTags = new List<string> ();
				bool isStatusLine = false;
				foreach (string line in lines) {
					if (line.Contains ("STATUS>") || isStatusLine) {
						if (!line.Contains ("</STATUS>")) {
							statusTags.Add (line);
						}

						//only handle 1 status message
						if (line.Contains ("</STATUS>")) {
							break;
						} else {
							isStatusLine = true;
						}
					} else {
						continue;
					}
				}

				var balanceTags = from line in lines
                              where line.Contains ("<LEDGERBAL")
					|| line.Contains ("<AVAILBAL>")
					|| line.Contains ("<DTASOF>")
					|| line.Contains ("<BALAMT>")
                              select line;

				List<string> transactionTags = new List<string> ();
				bool isTransactionLine = false;
				foreach (string line in lines) {
					if (line.Contains ("STMTTRN>") || isTransactionLine) {
						if (!line.Contains ("</STMTTRN>")) {
							transactionTags.Add (line);
						}

						isTransactionLine = !line.Contains ("</STMTTRN>");
					} else {
						continue;
					}
				}

				List<string> accountInfoTags = new List<string> ();
				bool isAcctInfoLine = false;
				bool isCreditCardAccount = false;
				foreach (string line in lines) {
					if (line.Contains ("ACCTINFO>") || isAcctInfoLine) {
						if (line.Contains("<CCACCTINFO")){
							accountInfoTags.Add ("<ACCTTYPE>CREDITCARD");
						}
						else if (line.Contains ("<ACCTINFO>")
							|| line.Contains ("<DESC")
							|| line.Contains ("<BANKID")
							|| line.Contains ("<ACCTID")
							|| line.Contains ("<ACCTTYPE")) {
							accountInfoTags.Add (line);
						}

						isAcctInfoLine = !line.Contains ("</ACCTINFO>");
					} else {
						continue;
					}
				}

				XElement child = null;

				// parse status
				if (statusTags.Count > 0) {
					XElement statusElement = new XElement ("STATUS");
					root.Add (statusElement);

					foreach (var line in statusTags) {
						if (!line.Contains ("STATUS>")) {
							var tagName = GetTagName (line);
							var elementChild = new XElement (tagName) {
								Value = GetTagValue(line)
							};

							statusElement.Add (elementChild);
						}
					}
				}

				var status = root.Element ("STATUS");
				if (status != null) {
					var codeElement = status.Element ("CODE");
					if (codeElement != null 
					    && !string.IsNullOrEmpty(codeElement.Value)
					    && codeElement.Value != STATUS_CODE_SUCCESS){

						string message = string.Empty;
						var messageElement = status.Element ("MESSAGE");
						if (messageElement != null && messageElement.Value != null){
							message = messageElement.Value;
						}
						   
						throw new OfxStatusException(codeElement.Value, message);
					}
				}
				
					// parse transactions
				if (transactionTags.Count > 0) {
					//balances
					foreach (var line in balanceTags) {
						if (line.Contains ("<LEDGERBAL") || line.Contains ("<AVAILBAL")) {
							child = new XElement (GetTagName (line));
							root.Add (child);
						} else {
							var tagName = GetTagName (line);
							var elementChild = new XElement (tagName) {
								Value = GetTagValue(line)
							};

							child.Add (elementChild);
						}
					}

					XElement transactions = new XElement ("BANKTRANLIST");
					root.Add (transactions);

					foreach (var line in transactionTags) {
						if (line.Contains ("<STMTTRN>")) {
							child = new XElement ("STMTTRN");
							transactions.Add (child);
						} else if (!line.Contains ("</STMTTRN>")) {
							var tagName = GetTagName (line);
							var elementChild = new XElement (tagName) {
								Value = GetTagValue(line)
							};

							child.Add (elementChild);
						}
					}
				}

				// parse account list
				if (accountInfoTags.Count > 0) {
					XElement accounts = new XElement ("ACCTINFORS");
					root.Add (accounts);

					foreach (var line in accountInfoTags) {
						if (line.Contains ("<ACCTINFO>")) {
							child = new XElement ("ACCTINFO");
							accounts.Add (child);
						} else if (!line.Contains ("</ACCTINFO>")) {
							var tagName = GetTagName (line);
							var elementChild = new XElement (tagName) {
								Value = GetTagValue(line)
							};

							child.Add (elementChild);
						}
					}
				}
			} catch (OfxStatusException){
				throw;
			} catch (OfxParseException) {
				throw;
			} catch (Exception ex) {
				throw new OfxParseException (ex.Message, ex);
			}

			return root;
		}

		/// <summary>
		/// Get the Tag name to create an Xelement
		/// </summary>
		/// <param name="line">One line from the file</param>
		/// <returns></returns>
		private static string GetTagName (string line)
		{
			int pos_init = line.IndexOf ("<") + 1;
			int pos_end = line.IndexOf (">");
			pos_end = pos_end - pos_init;
			return line.Substring (pos_init, pos_end);
		}

		/// <summary>
		/// Get the value of the tag to put on the Xelement
		/// </summary>
		/// <param name="line">The line</param>
		/// <returns></returns>
		private static string GetTagValue (string line)
		{
			int pos_init = line.IndexOf (">") + 1;
			string retValue = line.Substring (pos_init).Trim ();

			// the date contains a time zone offset
			if (retValue.IndexOf ("[") != -1) {
				// Date - get only 8 date digits
				retValue = retValue.Substring (0, 8);
			}

			// if the value is exactly 18 digits and the 14th digit is a dot
			// this should be a date - trim it
			if (retValue.Length == 18 && retValue.IndexOf (".") == 14) {
				// Date - get only 8 date digits
				retValue = retValue.Substring (0, 8);
			}

			return retValue;
		}
	}
}
