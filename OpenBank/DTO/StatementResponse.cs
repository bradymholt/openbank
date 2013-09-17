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
        public StatementBalance LedgerBalance { get; set; }
        public StatementBalance AvailableBalance { get; set; }
        public List<StatementTransaction> Transactions { get; set; }
    }
}
