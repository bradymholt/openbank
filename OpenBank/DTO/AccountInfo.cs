using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    /// <summary>
    /// ACCTINFO
    /// </summary>
    public class AccountInfo
    {
        public string Description { get; set; }
        public string BankID { get; set; }
        public string AccountID { get; set; }
        public string AccountType { get; set; }
        public string Status { get; set; }
    }
}
