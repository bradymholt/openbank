using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class StatementRequestParameters : OfxRequestParameters
    {
        public string BankID { get; set; }
        public string AccountID { get; set; }
        public AccountType AccountType { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
