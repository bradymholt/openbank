using System;
using System.Collections.Generic;

namespace OpenBank.DTO
{
	public class AccountsResponse : DTO.ServiceResponse
	{
		/// <summary>
		/// ACCTINFOTRNRS
		/// </summary>
		public List<DTO.Account> accounts { get; set; }
	}
}

