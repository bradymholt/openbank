using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml.Linq;
using System.Reflection;

namespace OpenBank.OfxAssimilate
{
	public abstract class OfxFetcher
	{
		public OfxFetcher (OfxFetchParameters parameters)
		{
			m_parameters = parameters;
		}

		private const string IDENTIFIER_CHARS_STRING = "0123456789";
		private static readonly char[] IDENTIFIER_CHARS = IDENTIFIER_CHARS_STRING.ToCharArray ();
		private OfxFetchParameters m_parameters;

		public string OfxRequestBody { get; protected set; }

		public string OfxResponseBody { get; protected set; }

		public XElement ParsedOfx { get; set; }

		public OfxResponse Response { get; set; }

		private const string OFX_REQUEST =
              "OFXHEADER:100\n"
			+ "DATA:OFXSGML\n"
			+ "VERSION:102\n"
			+ "SECURITY:NONE\n"
			+ "ENCODING:USASCII\n"
			+ "CHARSET:1252\n"
			+ "COMPRESSION:NONE\n"
			+ "OLDFILEUID:NONE\n"
			+ "NEWFILEUID:{0}\n\n"
			+ "<OFX>\n"
			+ " <SIGNONMSGSRQV1>\n"
			+ "  <SONRQ>\n"
			+ "   <DTCLIENT>{1}:GMT\n"
			+ "   <USERID>{2}\n"
			+ "   <USERPASS>{3}\n"
			+ "   <LANGUAGE>ENG\n"
			+ "   <FI>\n"
			+ "    <ORG>{4}\n"
			+ "    <FID>{5}\n"
			+ "   </FI>\n"
			+ "   <APPID>QWIN\n"
			+ "   <APPVER>1700\n"
			+ "  </SONRQ>\n"
			+ " </SIGNONMSGSRQV1>\n"
			+ " {6}\n"
			+ "</OFX>";

		protected abstract string BuildRequestInnerBody ();

		protected abstract OfxResponse BuildResponse (XElement parsedOfx);

		public bool Fetch ()
		{
			try {
				//request body
				this.OfxRequestBody = this.BuildOfxRequest ();

				//get raw ofx response
				this.OfxResponseBody = SendRequestAndGetResponse (this.OfxRequestBody, m_parameters.OFX_URL);

				//parse ofx to xml
				OfxToXmlParser parser = new OfxToXmlParser (this.OfxResponseBody);
				this.ParsedOfx = parser.Parse ();

				//build response
				this.Response = this.BuildResponse (this.ParsedOfx);

			} catch (ParametersMissingException pmex){
				this.Response = new OfxResponseError (HttpStatusCode.BadRequest) {
					friendly_error = "The bank could not be contacted because of missing information.",
					detailed_error = string.Concat("Missing parameter(s): ", string.Join(", ", pmex.MissingNames))
				};
			} catch (System.UriFormatException uriEx){
				this.Response = new OfxResponseError (HttpStatusCode.BadRequest) {
					friendly_error = "The bank could not be contacted.",
					detailed_error = uriEx.Message
				};
			} catch (WebException e) {
				using (WebResponse response = e.Response) {
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					using (Stream data = response.GetResponseStream())
					using (var reader = new StreamReader(data)) {
						this.OfxResponseBody = reader.ReadToEnd ();
						this.Response = new OfxResponseError ((HttpStatusCode)((int)httpResponse.StatusCode)) {
							friendly_error = "An error occured when communicating with the bank.",
							detailed_error = this.OfxResponseBody
						};
					}
				}
			} catch (OfxStatusException statusEx){
				this.Response = new OfxResponseError (HttpStatusCode.BadRequest) {
					friendly_error =  string.Concat("Bank response: " + statusEx.Message),
					detailed_error = string.Format("{0} ({1})", statusEx.Message, statusEx.Code)
				};
			} catch(OfxParseException pex){
				this.Response = new OfxResponseError (HttpStatusCode.InternalServerError) {
					friendly_error = "An error occured when reading the response from the bank.",
					detailed_error = pex.Message
				};
			} catch(Exception ex){
				this.Response = new OfxResponseError (HttpStatusCode.InternalServerError) {
					friendly_error = "An error occured when processing the response from the bank.",
					detailed_error = ex.Message  
				};
			}

			return !(this.Response is OfxResponseError);
		}

		protected string BuildOfxRequest ()
		{
			EnsureAllParametersAvailable ();

			string requestInnerBody = BuildRequestInnerBody ();
			string requestBody = string.Format (OFX_REQUEST,
			                                    GenerateRandomString (IDENTIFIER_CHARS, 32),
			                                    DateTime.UtcNow.ToString ("yyyyMMddHHmmss.fff"), //UTC
			                                    m_parameters.UserID,
			                                    m_parameters.Password,
			                                    m_parameters.ORG,
			                                    m_parameters.FID,
			                                    requestInnerBody);

			return requestBody;
		}

		public void EnsureAllParametersAvailable(){
			List<string> missingParameters = new List<string> ();
			foreach (PropertyInfo pi in m_parameters.GetType().GetProperties()) {
				object value = pi.GetValue (m_parameters, null);
				if (value == null || string.IsNullOrEmpty (value.ToString())) {
					missingParameters.Add (pi.Name);
				}
			}

			if (missingParameters.Count > 0) {
				throw new ParametersMissingException (missingParameters);
			}
		}

		public string SendRequestAndGetResponse (string requestBody, string ofx_url)
		{
			string responseBody = string.Empty;

			System.Net.ServicePointManager.Expect100Continue = false; //otherwise 'Expect: 100-continue' header is added to request which many ofx servers do not like.

			WebRequest webRequest = WebRequest.Create (ofx_url);
			webRequest.ContentType = "application/x-ofx";
			webRequest.Method = "POST";

			string proxyUrl = ConfigurationManager.AppSettings ["proxy_url"];
			if (!string.IsNullOrEmpty (proxyUrl)) {
				WebProxy proxy = new WebProxy (proxyUrl);
				proxy.UseDefaultCredentials = true;
				webRequest.Proxy = proxy;
			}

			byte[] contentBytes = Encoding.ASCII.GetBytes (requestBody);
			webRequest.ContentLength = contentBytes.Length;

			using (Stream os = webRequest.GetRequestStream()) {
				os.Write (contentBytes, 0, contentBytes.Length);
			}

			HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse ();
			if (httpResponse != null) {
				StreamReader sr = new StreamReader (httpResponse.GetResponseStream ());
				responseBody = sr.ReadToEnd ();
			}
		
			return responseBody;
		}

		public static string GenerateRandomString (int length)
		{
			return GenerateRandomString (IDENTIFIER_CHARS, length);
		}

		public static string GenerateRandomString ()
		{
			return GenerateRandomString (IDENTIFIER_CHARS, 8);
		}

		public static string GenerateRandomString (char[] allowedCharacters, int length)
		{
			string newCode = null;
			byte[] randomBytes = new byte[length];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider ();
			rng.GetBytes (randomBytes);
			char[] chars = new char[length];
			int countAllowedCharacters = allowedCharacters.Length;

			for (int i = 0; i < length; i++) {
				int currentRandomNumber = Convert.ToInt32 (randomBytes [i]);
				chars [i] = allowedCharacters [currentRandomNumber % countAllowedCharacters];
			}

			newCode = new string (chars);
			return newCode;
		}
	}
}
