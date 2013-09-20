using System;
using System.Net;

namespace OpenBank.OfxAssimilate
{
	public class OfxResponse
	{
		public OfxResponse(HttpStatusCode status)
		{
			this.HttpStatus = status;
		}

		public OfxResponse()
			:this(HttpStatusCode.OK)
		{
		}

		internal HttpStatusCode HttpStatus {get; set; }
	}
}

