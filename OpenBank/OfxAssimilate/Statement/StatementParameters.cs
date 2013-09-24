using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.OfxAssimilate
{
	public class StatementParameters : OfxFetchParameters
    {
        public string BankCode { get; set; }

		[ParameterRequired]
        public string AccountID { get; set; }

		[ParameterRequired]
        public string AccountType { get; set; }

		[ParameterRequired]
        public DateTime DateStart { get; set; }

		[ParameterRequired]
		public DateTime DateEnd { get; set; }
    }
}
