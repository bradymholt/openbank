using System;

namespace OpenBank.OfxAssimilate
{
	public class StatementResponse : OfxResponse
	{
		/// <summary>
		/// BANKMSGSRSV1, CREDITCARDMSGSRSV1
		/// </summary>
		public OfxData.OfxStatement statement { get; set; }
	}
}

