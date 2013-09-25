using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenBank.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Use accounts.sensitive.template as an example
            // In this git repo, any file ending in ".sensitive" is ignored.
            JObject hash = JsonConvert.DeserializeObject(File.ReadAllText("../../accounts.sensitive")) as JObject;
            var postParameters = hash.First.First.ToObject<Dictionary<string, string>>();

            StringBuilder postData = new StringBuilder();
            foreach(var kvp in postParameters)
                if(String.IsNullOrEmpty(postParameters[kvp.Key]) == false)
                    postData.AppendFormat("{0}={1}&", kvp.Key, kvp.Value);

            byte[] data = Encoding.ASCII.GetBytes(postData.ToString().TrimEnd('&'));

            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create("http://localhost:1234/statement");
            httpReq.Method = "POST";
            httpReq.ContentType = "application/x-www-form-urlencoded";
            httpReq.ContentLength = data.Length;
            httpReq.Accept = "application/json";

            using (var stream = httpReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)httpReq.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseString);
            Console.ReadLine();
        }
    }
}