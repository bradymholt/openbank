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
	public abstract class ScrapeFetcher
	{
		private static NLog.Logger s_logger = NLog.LogManager.GetLogger("*");

		private ScrapeFetchParameters m_params;
		public ScrapeFetcher (ScrapeFetchParameters parameters)
		{
			m_params = parameters;
		}

		public OfxAssimilate.OfxResponse Response { get; set; }

		protected abstract string GetScriptName (string fid);
		protected abstract void PrepScrape (Process process);
		protected abstract OfxAssimilate.OfxResponse ProcessScrape (string outputPath);

		public bool Fetch()
		{
			string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string casperJsPath = Path.Combine(assemblyPath, "casperjs");
			string scriptsPath = Path.Combine(assemblyPath, "casperjs/scripts");
			string outputPath = Path.Combine(assemblyPath, "casperjs/output");
			string workingOutputPath = Path.Combine(outputPath, Guid.NewGuid().ToString());
			Directory.CreateDirectory(workingOutputPath);

			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "/usr/local/bin/phantomjs";
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.WorkingDirectory = casperJsPath;
			startInfo.Arguments = Path.Combine(scriptsPath, GetScriptName(m_params.FID));
			startInfo.Arguments += " " + casperJsPath;
			startInfo.Arguments += " --user_id=" + m_params.UserID;
			startInfo.Arguments += " --password=" + m_params.Password;
			startInfo.Arguments += " --security_answers=" + m_params.SecurityAnswers;
			startInfo.Arguments += " --output_path=" + workingOutputPath;

			if (ConfigurationManager.AppSettings["phantomJsDebugMode"] == "true")
			{
				startInfo.Arguments += " --debug=true";

				startInfo.UseShellExecute = false;
				startInfo.CreateNoWindow = true;
				startInfo.RedirectStandardOutput = true;
				startInfo.RedirectStandardError = true;
			}

			try
			{
				// Start the process with the info we specified.
				// Call WaitForExit and then the using statement will close.
				using (Process exeProcess = new Process())
				{
					exeProcess.StartInfo = startInfo;
					exeProcess.OutputDataReceived += new DataReceivedEventHandler(exeProcess_OutputDataReceived);
					exeProcess.ErrorDataReceived += new DataReceivedEventHandler(exeProcess_ErrorDataReceived);

					PrepScrape(exeProcess);

					s_logger.Info("Starting phantomjs process: {0} {1}", startInfo.FileName, startInfo.Arguments);
					exeProcess.Start();

					exeProcess.BeginOutputReadLine();
					exeProcess.BeginErrorReadLine();
					exeProcess.WaitForExit();
				}

				this.Response = ProcessScrape(workingOutputPath);

			} catch (Exception ex) {
				this.Response = new OfxAssimilate.OfxResponseError (HttpStatusCode.InternalServerError) {
					friendly_error = "An error occured when processing the response from the bank.",
					detailed_error = ex.Message  
				};
			}
			finally
			{
				if (ConfigurationManager.AppSettings["phantomJsDebugMode"] != "true")
				{
					//Directory.Delete(workingOutputPath);
				}
			}

			return !(this.Response is OfxAssimilate.OfxResponseError);
		}

		void exeProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			s_logger.Debug (e.Data);
		}

		void exeProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			s_logger.Debug (e.Data);
		}
	}
}

