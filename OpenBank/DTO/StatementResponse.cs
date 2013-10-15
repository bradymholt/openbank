using System;

namespace OpenBank.DTO
{
	public class StatementResponse : DTO.ServiceResponse
	{
		/// <summary>
		/// BANKMSGSRSV1, CREDITCARDMSGSRSV1
		/// </summary>
		public DTO.Statement statement { get; set; }
	}
}

