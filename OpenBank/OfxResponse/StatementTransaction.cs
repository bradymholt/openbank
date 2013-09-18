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
        /// <summary>
        /// FITID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// NAME
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// TRNTYPE
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// DTPOSTED
        /// </summary>
        public DateTime? date { get; set; }

        /// <summary>
        /// TRNAMT
        /// </summary>
        public decimal? amount { get; set; }
    }
}
