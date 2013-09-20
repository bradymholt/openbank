using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.OfxData
{
    /// <summary>
    /// BAL
    /// </summary>
    public class OfxStatementBalance
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
