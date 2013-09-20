using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;

namespace OpenBank.Service
{
	public class DiagnosticsConfigBootstrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = System.Configuration.ConfigurationManager.AppSettings["dashboard_password"] }; }
        }
    }
}
