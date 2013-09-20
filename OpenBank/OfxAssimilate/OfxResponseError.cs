using System;
using System.Net;

namespace OpenBank.OfxAssimilate
{
	public class OfxResponseError : OfxResponse
	{
		public OfxResponseError(HttpStatusCode status)
			:base(status){
		}

		public string detailed_error {get;set;}
		public string friendly_error {get;set;}
	}
}

