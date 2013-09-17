using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    /// <summary>
    /// STMTTRN
    /// </summary>
    public class StatementTransaction
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime? DatePosted { get; set; }
        public decimal? Amount { get; set; }
    }
}
