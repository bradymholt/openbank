using System;

namespace OpenBank.OfxAssimilate
{
	public class OfxParseException : ApplicationException
	{
		public OfxParseException (string message, Exception innerException)
			:base(message, innerException)
		{
		}

		public OfxParseException (string message)
		:base(message){
		}
	}
}

