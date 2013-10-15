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

namespace OpenBank.FetchScrape
{
	public class ScrapeStatementFetcher : ScrapeFetcher
	{
		private ScrapeStatementParameters m_statementParameters;
		public ScrapeStatementFetcher (ScrapeStatementParameters parameters)
			:base(parameters)
		{
			m_statementParameters = parameters;
		}

		protected override string GetScriptName (string fid)
		{
			return string.Format("{0}_statement.js", fid);
		}

		protected override void PrepScrape (System.Diagnostics.Process process)
		{
			process.StartInfo.Arguments += string.Format(" --account_id=\"{0}\"", m_statementParameters.AccountID);
			process.StartInfo.Arguments += string.Format(" --date_start={0}", m_statementParameters.DateStart.ToString("MM/dd/yyyy"));
			process.StartInfo.Arguments += string.Format(" --date_end={0}", m_statementParameters.DateEnd.ToString("MM/dd/yyyy"));
		}

		protected override DTO.ServiceResponse ProcessScrape (string outputPath, string debugID)
		{
			DTO.ServiceResponse response = null;
			string statementFilePath = Path.Combine (outputPath, "statement.qfx");
			if (File.Exists(statementFilePath)) {
				string statementOfx = File.ReadAllText(statementFilePath);
				Parse.OfxToXmlParser parser = new Parse.OfxToXmlParser (statementOfx);
				XElement parsedOfx = parser.Parse ();

				Parse.OfxResponseBuilder responseBuilder = new Parse.OfxResponseBuilder();
				response = responseBuilder.BuildStatementResponse (parsedOfx, m_statementParameters.DateStart, m_statementParameters.DateEnd);
			} else {
				response = new DTO.ResponseError (HttpStatusCode.BadRequest) {
					friendly_error = "An error occured when atempting to get account statement.",
					detailed_error = "statement file missing" 
				};
			}

			return response;
		}
	}
}

