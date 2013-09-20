using System;
using System.Collections.Generic;

namespace OpenBank.OfxAssimilate
{
	public class ParametersMissingException : ApplicationException
	{
		public List<string> MissingNames;
		public ParametersMissingException (List<string> missingNames)
		{
			this.MissingNames = missingNames;
		}
	}
}

