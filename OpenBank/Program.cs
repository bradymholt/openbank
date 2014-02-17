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
            Console.WriteLine();
			Console.Write("Starting up...");

			System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidator;

			string port = ConfigurationManager.AppSettings["port"];
            using (var nancyHost = new Nancy.Hosting.Self.NancyHost(new Uri(string.Concat("http://localhost:", port))))
            {
                nancyHost.Start();
                Console.WriteLine("success!");
                
                Console.WriteLine(string.Format("Server listening on port {0} and waiting for connections.", port));
                Console.WriteLine(string.Format("Navigate to http://localhost:{0}/ for help.", port));

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
                Console.WriteLine();
                Console.WriteLine("[Press any key to exit]");
                Console.ReadLine();
            }
        }

		public static bool CertificateValidator(object sender,
		                                        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
		                                        System.Security.Cryptography.X509Certificates.X509Chain chain,
		                                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			//trust all certificates; needed for Mono compatibility
			return true;
		}
    }
}
