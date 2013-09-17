using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml.Linq;

namespace OpenBank
{
    public abstract class OfxRequestor
    {
        private OfxRequestorParameters m_parameters;

        public OfxRequestor(OfxRequestorParameters parameters)
        {
            m_parameters = parameters;
        }

        private const string IDENTIFIER_CHARS_STRING = "0123456789";
        private readonly char[] IDENTIFIER_CHARS = IDENTIFIER_CHARS_STRING.ToCharArray();

        public string RequestBody { get; protected set; }
        public string ResponseBody { get; protected set; }
        public HttpWebResponse Response { get; set; }
       
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

        public OfxResponse FetchOfx()
        {
            this.GetResponse();

            OfxParser parser = new OfxParser();
            OfxResponse ofxResponse = parser.Parse(this.ResponseBody);

            return ofxResponse;
        }

        public string GetResponse()
        {
            this.RequestBody = BuildRequestBody();
            this.ResponseBody = SendRequestGetResponse(m_parameters.URL, this.RequestBody);
            return this.ResponseBody;
        }

        private string SendRequestGetResponse(string url, string request)
        {
            System.Net.ServicePointManager.Expect100Continue = false; //otherwise 'Expect: 100-continue' header is added to request

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/x-ofx";
            webRequest.Method = "POST";

            string proxyUrl = ConfigurationManager.AppSettings["proxy_url"];
            if (!string.IsNullOrEmpty(proxyUrl))
            {
                WebProxy proxy = new WebProxy(proxyUrl);
                proxy.UseDefaultCredentials = true;
                webRequest.Proxy = proxy;
            }

            byte[] contentBytes = Encoding.ASCII.GetBytes(request);
            webRequest.ContentLength = contentBytes.Length;

            using (Stream os = webRequest.GetRequestStream())
            {
                os.Write(contentBytes, 0, contentBytes.Length);
            }

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            string responseBody = string.Empty; 
            if (webResponse != null)
            {
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                responseBody = sr.ReadToEnd();
            }

            this.Response = webResponse;

            return responseBody;
        }

        protected string BuildRequestBody()
        {
            string requestBody = string.Format(OFX_REQUEST,
                GenerateRandomString(IDENTIFIER_CHARS, 32),
                DateTime.UtcNow.ToString("yyyyMMddHHmmss.fff"), //UTC
                m_parameters.UserID,
                m_parameters.Password,
                m_parameters.ORG,
                m_parameters.FID,
                BuildInnerRequest());

            return requestBody;
        }

        protected abstract string BuildInnerRequest();

        protected string GenerateRandomString(int length)
        {
            return GenerateRandomString(IDENTIFIER_CHARS, length);
        }

        protected string GenerateRandomString()
        {
            return GenerateRandomString(IDENTIFIER_CHARS, 8);
        }

        protected string GenerateRandomString(char[] allowedCharacters, int length)
        {
            string newCode = null;
            byte[] randomBytes = new byte[length];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            char[] chars = new char[length];
            int countAllowedCharacters = allowedCharacters.Length;

            for (int i = 0; i < length; i++)
            {
                int currentRandomNumber = Convert.ToInt32(randomBytes[i]);
                chars[i] = allowedCharacters[currentRandomNumber % countAllowedCharacters];
            }

            newCode = new string(chars);
            return newCode;
        }
    }
}
