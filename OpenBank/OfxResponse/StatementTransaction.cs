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
        public string ID { get; set; }

        /// <summary>
        /// NAME
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// TRNTYPE
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// DTPOSTED
        /// </summary>
        public DateTime? DatePosted { get; set; }

        /// <summary>
        /// TRNAMT
        /// </summary>
        public decimal? Amount { get; set; }
    }
}
