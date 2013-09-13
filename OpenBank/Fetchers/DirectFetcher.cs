using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;

namespace OfxClient
{
    public class DirectFetcher : IFetchOfx
    {
        public DirectFetcher(OfxRequest request)
        {
            m_Request = request;
        }

        private const string IDENTIFIER_CHARS_STRING = "0123456789";
        private readonly char[] IDENTIFIER_CHARS = IDENTIFIER_CHARS_STRING.ToCharArray();

        private OfxRequest m_Request;

        public string Request { get; protected set; }
        public string Response { get; protected set; }

        #region OFX Request Constants
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

        private const string OFX_BANKING_STATEMENT_REQUEST =
              "<BANKMSGSRQV1>\n"
            + " <STMTTRNRQ>\n"
            + "  <TRNUID>{0}\n"
            + "  <CLTCOOKIE>{1}\n"
            + "  <STMTRQ>\n"
            + "   <BANKACCTFROM>\n"
            + "    <BANKID>{2}\n"
            + "    <ACCTID>{3}\n"
            + "    <ACCTTYPE>{4}\n"
            + "   </BANKACCTFROM>\n"
            + "   <INCTRAN>\n"
            + "    <DTSTART>{5}\n"
            + "    <INCLUDE>Y\n"
            + "   </INCTRAN>\n"
            + "  </STMTRQ>\n"
            + " </STMTTRNRQ>\n"
            + "</BANKMSGSRQV1>";

        private const string OFX_CREDIT_CARD_STATEMENT_REQUEST =
              "<CREDITCARDMSGSRQV1>\n"
            + " <CCSTMTTRNRQ>\n"
            + "  <TRNUID>{0}\n"
            + "  <CLTCOOKIE>{1}\n"
            + "  <CCSTMTRQ>\n"
            + "   <CCACCTFROM>\n"
            + "    <ACCTID>{2}\n"
            + "   </CCACCTFROM>\n"
            + "   <INCTRAN>\n"
            + "    <DTSTART>{3}\n"
            + "    <INCLUDE>Y\n"
            + "   </INCTRAN>\n"
            + "  </CCSTMTRQ>\n"
            + " </CCSTMTTRNRQ>\n"
            + "</CREDITCARDMSGSRQV1>";
        #endregion

        public OfxAccount FetchOfx()
        {
            string response = FetchOfx(m_Request);

            OfxParser parser = new OfxParser();
            OfxAccount ofxAcct = parser.LoadAccount(response);
            return ofxAcct;
        }

        private string FetchOfx(OfxRequest request)
        {
            this.Request = GenerateRequestBody(request);
            return FetchOfx(request.Url, this.Request);
        }

        public string FetchOfx(string url, string requestBody)
        {
            System.Net.ServicePointManager.Expect100Continue = false; //otherwise 'Expect: 100-continue' header is added to request

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/x-ofx";
            webRequest.Method = "POST";

            string proxyUrl = ConfigurationManager.AppSettings["proxyUrl"];
            if (!string.IsNullOrEmpty(proxyUrl))
            {
                WebProxy proxy = new WebProxy(proxyUrl);
                proxy.UseDefaultCredentials = true;
                webRequest.Proxy = proxy;
            }

            byte[] contentBytes = Encoding.ASCII.GetBytes(requestBody);
            webRequest.ContentLength = contentBytes.Length;

            using (Stream os = webRequest.GetRequestStream())
            {
                os.Write(contentBytes, 0, contentBytes.Length);
            }

            WebResponse webResponse = webRequest.GetResponse();

            if (webResponse != null)
            {
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                this.Response = sr.ReadToEnd();
            }

            return this.Response;
        }

        public string GenerateRequestBody(OfxRequest request)
        {
            string transactionRequest = string.Empty;
            switch (request.AccountType)
            {
                case OfxAccountType.CREDITCARD:
                    transactionRequest = string.Format(OFX_CREDIT_CARD_STATEMENT_REQUEST,
                        GenerateRandomString(IDENTIFIER_CHARS, 8),
                        GenerateRandomString(IDENTIFIER_CHARS, 5),
                        request.AccountID,
                        request.DateStart.ToString("yyyyMMddHHmmss.fff")); //UTC
                    break;
                default:
                    transactionRequest = string.Format(OFX_BANKING_STATEMENT_REQUEST,
                        GenerateRandomString(IDENTIFIER_CHARS, 8),
                        GenerateRandomString(IDENTIFIER_CHARS, 5),
                        request.BankID,
                        request.AccountID,
                        request.AccountType.ToString(),
                        request.DateStart.ToString("yyyyMMddHHmmss.fff")); //UTC
                    break;

            }

            string requestBody = string.Format(OFX_REQUEST,
                GenerateRandomString(IDENTIFIER_CHARS, 32),
                request.DateEnd.ToString("yyyyMMddHHmmss.fff"), //UTC
                request.UserID,
                request.UserPassword,
                request.Org,
                request.Fid,
                transactionRequest);

            return requestBody;
        }

        private static string GenerateRandomString(char[] allowedCharacters, int length)
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
