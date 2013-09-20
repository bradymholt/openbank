using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.OfxAssimilate
{
	public class StatementParameters : OfxFetchParameters
    {
        public string BankID { get; set; }
        public string AccountID { get; set; }
        public OfxData.OfxAccountType AccountType { get; set; }

        public DateTime DateStart { get; set; }
		public DateTime DateEnd { get; set; }
    }
}
