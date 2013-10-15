using System;

namespace OpenBank.Parse
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

