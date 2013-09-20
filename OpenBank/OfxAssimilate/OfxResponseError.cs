using System;

namespace OpenBank.OfxAssimilate
{
	public class OfxResponseError : OfxResponse
	{
		public string friendly_error {get;set;}
		public string detailed_error {get;set;}
		public int http_status_code {get; set; }
	}
}

