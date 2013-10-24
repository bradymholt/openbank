using System;
using System.Net;

namespace OpenBank.DTO
{
	public class ResponseError : DTO.ServiceResponse
	{
		public ResponseError(HttpStatusCode status)
			:base(status){
		}

		public string detailed_error {get;set;}
		public string friendly_error {get;set;}
		public bool is_security_question_asked {get;set;}
	}
}

