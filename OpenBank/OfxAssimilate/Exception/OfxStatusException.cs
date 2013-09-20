using System;

namespace OpenBank
{
	public class OfxStatusException : ApplicationException
	{
		public string Code { get; set;}
		public string Message {get;set;}

		public OfxStatusException (string code, string message)
		{
			this.Code = code;
			this.Message = message;
		}
	}
}

