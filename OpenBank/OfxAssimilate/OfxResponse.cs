using System;
using System.Net;

namespace OpenBank.OfxAssimilate
{
	public class OfxResponse
	{
		public OfxResponse(HttpStatusCode status)
		{
			this.HttpStatus = (int)status;
		}

		public OfxResponse(int statusCode)
		{
			this.HttpStatus = statusCode;
		}

		public OfxResponse()
			:this((int)HttpStatusCode.OK)
		{
		}

		internal int HttpStatus {get; set; }
	}
}

