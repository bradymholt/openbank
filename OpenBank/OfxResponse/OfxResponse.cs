using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    /// <summary>
    /// OFX
    /// </summary>
    public class OfxResponse
    {
        /// <summary>
        /// ACCTINFOTRNRS
        /// </summary>
        public List<AccountInfo> Accounts { get; set; }

        /// <summary>
        /// BANKMSGSRSV1, CREDITCARDMSGSRSV1
        /// </summary>
        public StatementResponse Statement { get; set; }
    }
}
