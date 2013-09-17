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
        /// <summary>
        /// DESC
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// BANKID
        /// </summary>
        public string BankID { get; set; }

        /// <summary>
        /// ACCTID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// ACCTTYPE
        /// </summary>
        public string AccountType { get; set; }
    }
}
