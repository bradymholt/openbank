using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBank
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--OpenBank--");
            Console.WriteLine("Starting up...");
            string port = ConfigurationManager.AppSettings["service_port"];
            using (var nancyHost = new Nancy.Hosting.Self.NancyHost(new Uri(string.Concat("http://localhost:", port))))
            {
                nancyHost.Start();

                Console.WriteLine(string.Format("OpenBank server listening on port {0}.  Waiting for connections!", port));

                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

                nancyHost.Stop();
            }
        }
    }
}
