using System;
using System.Net;

namespace OpenBank.DTO
{
	public class ServiceResponse
	{
		public ServiceResponse(HttpStatusCode status)
		{
			this.HttpStatus = (int)status;
		}

		public ServiceResponse(int statusCode)
		{
			this.HttpStatus = statusCode;
		}

		public ServiceResponse()
			:this((int)HttpStatusCode.OK)
		{
		}

		internal int HttpStatus {get; set; }
	}
}

