using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfxClient
{
    public class OfxAccount
    {
        public string Organization { get; set; }
        public string BankID { get; set; }
        public string AccountID { get; set; }
        public string AccountType { get; set; }
        public decimal? LedgerBalanceAmount { get; set; }
        public DateTime? LedgerBalanceAsOfDate { get; set; }
        public decimal? AvailableBalanceAmount { get; set; }
        public DateTime? AvailableBalanceAsOfDate { get; set; }
        public IEnumerable<OfxTransaction> Transactions { get; set; }
    }
}
