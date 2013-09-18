using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    /// <summary>
    /// BAL
    /// </summary>
    public class StatementBalance
    {
        /// <summary>
        /// BALAMT
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// DTASOF
        /// </summary>
        public DateTime date { get; set; }
    }
}
