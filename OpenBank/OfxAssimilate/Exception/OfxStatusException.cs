using System;

namespace OpenBank
{
	public class OfxStatusException : ApplicationException
	{
		public string Code { get; set;}

		public OfxStatusException (string code, string message)
			:base(message)
		{
			this.Code = code;
		}
	}
}

