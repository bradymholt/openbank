using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfxClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string commandLineArgs = string.Join(" ", args);

            if (commandLineArgs.Contains("-s"))
            {
                Console.WriteLine("Entering service mode...");
                string port = ConfigurationManager.AppSettings["service_port"];
                using (var nancyHost = new Nancy.Hosting.Self.NancyHost(new Uri(string.Concat("http://localhost:", port))))
                {
                    nancyHost.Start();

                    Console.WriteLine(string.Format("Service listening on port {0}.", port));
                    Console.ReadLine();

                    nancyHost.Stop();
                }
            }
            else
            {
                Console.WriteLine("OfxClient: A client for Ofx servers.");
                Console.WriteLine("       Author: Brady Holt (http://www.geekytidbits.com)");
                Console.WriteLine();
                Console.WriteLine("Usage: ofxclient.exe [options]");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("   -s,                 Use service mode.");
                Console.WriteLine();
            }
        }
    }
}
