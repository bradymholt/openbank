using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    /// <summary>
    /// STMTRS
    /// </summary>
    public class StatementResponse
    {
        /// <summary>
        /// LEDGERBAL
        /// </summary>
        public StatementBalance LedgerBalance { get; set; }

        /// <summary>
        /// AVAILBAL
        /// </summary>
        public StatementBalance AvailableBalance { get; set; }

        /// <summary>
        /// BANKTRANLIST
        /// </summary>
        public List<StatementTransaction> Transactions { get; set; }
    }
}
