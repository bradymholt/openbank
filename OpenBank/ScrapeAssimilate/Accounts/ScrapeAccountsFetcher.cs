using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using NLog;


namespace OpenBank.ScrapeAssimilate
{
	public class ScrapeAccountsFetcher : ScrapeFetcher
	{
		public ScrapeAccountsFetcher (ScrapeAccountsParameters parameters)
			:base(parameters)
		{
		}

		protected override string GetScriptName (string fid)
		{
			return string.Format("{0}_accounts.js", fid);
		}

		protected override void PrepScrape (System.Diagnostics.Process process)
		{

		}
		
		protected override OpenBank.OfxAssimilate.OfxResponse ProcessScrape (string outputPath)
		{
			OfxAssimilate.OfxResponse response = null;
			string accountsFilePath = Path.Combine (outputPath, "accounts.json");
			if (File.Exists(accountsFilePath)) {
				string accountJson = File.ReadAllText(accountsFilePath);
				response = new OpenBank.OfxAssimilate.OfxAccountsResponse ();
				((OfxAssimilate.OfxAccountsResponse)response).accounts = JsonConvert.DeserializeObject<List<OfxData.OfxAccount>> (accountJson);
			} else {
				response = new OfxAssimilate.OfxResponseError (HttpStatusCode.BadRequest) {
					friendly_error = "An error occured when atempting to get list of accounts.",
					detailed_error = "accounts file missing" 
				};
			}

			return response;
		}
	}
}

