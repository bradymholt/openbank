using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nancy;
using Nancy.Routing;

namespace OpenBank.Service.Module
{
    public class Default : NancyModule
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
                    return GenerateParametersDescription(typeof(Accounts.Parameters));
                }
            }

            public string TransactionsServiceParameters
            {
                get
                {
                    return GenerateParametersDescription(typeof(Statement.Parameters));
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
                        description += string.Format(" [format: {0}]", ((ParameterFormat)parameterFormatAttributes[0]).Format);
                    }

					object[] parameterValuesAttributes = pi.GetCustomAttributes(typeof(ParameterValues), false);
					if (parameterValuesAttributes.Length > 0)
					{
						description += string.Format(" [values: {0}]", ((ParameterValues)parameterValuesAttributes[0]).Values);
					}

                    description += "<br/>";
                }
                return description;
            }
        }
    }
}
