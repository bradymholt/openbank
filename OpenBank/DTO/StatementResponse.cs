using System;
using System.Net;

namespace OpenBank.DTO
{
	public class StatementResponse : DTO.ServiceResponse
	{
		public StatementResponse() : base() {}
		public StatementResponse (HttpStatusCode status) : base (status)
		{
		}

		/// <summary>
		/// BANKMSGSRSV1, CREDITCARDMSGSRSV1
		/// </summary>
		public DTO.Statement statement { get; set; }
	}
}

