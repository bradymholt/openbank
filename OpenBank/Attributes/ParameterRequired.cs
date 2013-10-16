using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenBank
{
	public class ParameterRequired : Attribute
	{
		public ParameterRequired ()
		{
		}

		public static List<string> GetMissingParameters (object obj)
		{
			List<string> missingParameters = new List<string> ();
			foreach (PropertyInfo pi in obj.GetType().GetProperties()) {
				object value = pi.GetValue (obj, null);

				object[] parameterRequiredAttributes = pi.GetCustomAttributes (typeof(ParameterRequired), false);
				bool isRequired = parameterRequiredAttributes.Length > 0;

				if (isRequired && (value == null || string.IsNullOrEmpty (value.ToString ()))) {
					missingParameters.Add (pi.Name);
				}
			}

			return missingParameters;
		}
	}
}

