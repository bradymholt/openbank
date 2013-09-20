using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.OfxData
{
    /// <summary>
    /// ACCTINFO
    /// </summary>
    public class OfxAccount
    {
        /// <summary>
        /// DESC
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// BANKID
        /// </summary>
        public string bank_id { get; set; }

        /// <summary>
        /// ACCTID
        /// </summary>
        public string account_id { get; set; }

        /// <summary>
        /// ACCTTYPE
        /// </summary>
        public string account_type { get; set; }
    }
}
