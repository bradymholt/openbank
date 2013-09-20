using System;
using System.Collections.Generic;

namespace OpenBank.OfxAssimilate
{
	public class AccountsResponse : OfxResponse
	{
		/// <summary>
		/// ACCTINFOTRNRS
		/// </summary>
		public List<OfxData.OfxAccount> accounts { get; set; }
	}
}

