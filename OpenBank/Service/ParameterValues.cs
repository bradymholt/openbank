using System;

namespace OpenBank.Service
{
	public class ParameterValues : Attribute
	{
		public ParameterValues() {}
		public ParameterValues (string values)
			:this()
		{
			this.Values = values;
		}

		public string Values {get;set;}
	}
}

