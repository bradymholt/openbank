using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class OfxTransaction
    {
        public string TransactionID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
    }
}
