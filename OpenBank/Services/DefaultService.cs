using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nancy;
using Nancy.Routing;

namespace OpenBank.Service
{
    public class DefaultService : NancyModule
    {
        public DefaultService(IRouteCacheProvider routeCacheProvider)
        {
            Get["/"] = x =>
            {
                ViewModel model = new ViewModel();
                model.ServicePort = System.Configuration.ConfigurationManager.AppSettings["service_port"];

                return View["default", model];
            };
        }

        public class ViewModel
        {
            public string ServicePort { get; set; }
            public string AccountsServiceParameters
            {
                get
                {
                    return GenerateParametersDescription(typeof(AccountsService.Parameters));
                }
            }

            public string TransactionsServiceParameters
            {
                get
                {
                    return GenerateParametersDescription(typeof(TransactionsService.Parameters));
                }
            }

            private string GenerateParametersDescription(Type parametersType)
            {
                string description = string.Empty;
                foreach (PropertyInfo pi in parametersType.GetProperties())
                {
                    description += pi.Name;

                    object[] parameterFormatAttributes = pi.GetCustomAttributes(typeof(ParameterFormat), false);
                    if (parameterFormatAttributes.Length > 0)
                    {
                        description += string.Format(" (format: {0})", ((ParameterFormat)parameterFormatAttributes[0]).Format);
                    }

                    description += "<br/>";
                }
                return description;
            }
        }
    }
}
