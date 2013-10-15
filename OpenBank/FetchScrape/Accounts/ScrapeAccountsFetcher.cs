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

namespace OpenBank.FetchScrape
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
		
		protected override DTO.ServiceResponse ProcessScrape (string outputPath, string debugID)
		{
			DTO.ServiceResponse response = null;
			string accountsFilePath = Path.Combine (outputPath, "accounts.json");
			if (File.Exists(accountsFilePath)) {
				string accountJson = File.ReadAllText(accountsFilePath);
				response = new DTO.AccountsResponse ();
				((DTO.AccountsResponse)response).accounts = JsonConvert.DeserializeObject<List<DTO.Account>> (accountJson);
			} else {
				response = new DTO.ResponseError (HttpStatusCode.BadRequest) {
					friendly_error = "An error occured when atempting to get list of accounts.",
					detailed_error = string.Format("accounts file missing ({0})", debugID) 
				};
			}

			return response;
		}
	}
}

