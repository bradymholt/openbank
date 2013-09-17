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
            Console.WriteLine("--Welcome to OpenBank--");

            string port = ConfigurationManager.AppSettings["service_port"];
            using (var nancyHost = new Nancy.Hosting.Self.NancyHost(new Uri(string.Concat("http://localhost:", port))))
            {
                nancyHost.Start();

                Console.WriteLine(string.Format("Server listening on port {0}.  Waiting for connections...", port));

                Wait();
                
                nancyHost.Stop();
            }
        }

        static void Wait()
        {
            Type t = Type.GetType("Mono.Runtime");
            if (t != null)
            {
                //on mono, Console.ReadLine() will not keep the app running in background.
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
            else
            {
                Console.WriteLine("[Press any key to exit]");
                Console.ReadLine();
            }
        }
    }
}
