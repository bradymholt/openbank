using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.OfxData
{
    /// <summary>
    /// STMTRS
    /// </summary>
    public class OfxStatement
    {
        /// <summary>
        /// LEDGERBAL
        /// </summary>
        public OfxStatementBalance ledger_balance { get; set; }

        /// <summary>
        /// AVAILBAL
        /// </summary>
        public OfxStatementBalance available_balance { get; set; }

        /// <summary>
        /// BANKTRANLIST
        /// </summary>
        public List<OfxStatementTransaction> transactions { get; set; }
    }
}
