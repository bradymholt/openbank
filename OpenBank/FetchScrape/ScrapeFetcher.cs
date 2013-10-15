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
	public abstract class ScrapeFetcher
	{
		private const int MAX_WAIT_MILLISECONDS = 15000; //15 seconds

		private ScrapeFetchParameters m_params;
		private string m_assemblyPath;
		private string m_casperJsPath;
		private string m_scriptsPath;
		private string m_outputPath;
		private string m_workingID;
		private string m_workingOutputPath;
		private string m_outputData;
		private string m_errorData;

		public ScrapeFetcher (ScrapeFetchParameters parameters)
		{
			m_params = parameters;

			m_assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			m_casperJsPath = Path.Combine(m_assemblyPath, "casperjs");
			m_scriptsPath = Path.Combine(m_assemblyPath, "casperjs/scripts");
			m_outputPath = Path.Combine(m_assemblyPath, "casperjs/output");
			m_workingID = Guid.NewGuid ().ToString ();
			m_workingOutputPath = Path.Combine(m_outputPath, m_workingID);
		}

		public DTO.ServiceResponse Response { get; set; }

		protected abstract string GetScriptName (string fid);
		protected abstract void PrepScrape (Process process);
		protected abstract DTO.ServiceResponse ProcessScrape (string outputPath, string debugID);

		public bool Fetch()
		{
			Directory.CreateDirectory(m_workingOutputPath);

			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "/usr/local/bin/phantomjs";
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.WorkingDirectory = m_casperJsPath;
			startInfo.Arguments = Path.Combine(m_scriptsPath, GetScriptName(m_params.FID));
			startInfo.Arguments += " " + m_casperJsPath;
			startInfo.Arguments += string.Format(" --user_id=\"{0}\"", m_params.UserID);
			startInfo.Arguments += string.Format(" --password=\"{0}\"", m_params.Password);
			startInfo.Arguments += string.Format(" --security_answers=\"{0}\"", m_params.SecurityAnswers);
			startInfo.Arguments += string.Format(" --output_path=\"{0}\"", m_workingOutputPath);

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

					m_outputData += string.Format("[START] {0} {1}\n", startInfo.FileName, startInfo.Arguments);
					exeProcess.Start();

					exeProcess.BeginOutputReadLine();
					exeProcess.BeginErrorReadLine();
					exeProcess.WaitForExit(MAX_WAIT_MILLISECONDS);
				}

				this.Response = ProcessScrape(m_workingOutputPath, m_workingID);

			} catch (Exception ex) {
				this.Response = new DTO.ResponseError (HttpStatusCode.InternalServerError) {
					friendly_error = "An error occured when processing the response from the bank.",
					detailed_error = ex.Message  
				};
			}

			if (!(this.Response is DTO.ResponseError)) {
				Directory.Delete (m_workingOutputPath, true);
				return true;
			} else {
				File.WriteAllText(Path.Combine(m_workingOutputPath, "error.log"), m_errorData);
				File.WriteAllText(Path.Combine(m_workingOutputPath, "output.log"), m_outputData);
				return false;
			}
		}

		void exeProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			m_errorData += e.Data;
		}

		void exeProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			m_outputData += e.Data;
		}
	}
}

