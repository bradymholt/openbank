using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class StatementParameters : OfxRequestorParameters
    {
        public string BankID { get; set; }
        public string AccountID { get; set; }
        public AccountType AccountType { get; set; }

        public DateTime DateFrom { get; set; }
    }
}
