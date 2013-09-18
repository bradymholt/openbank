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
        public List<AccountInfo> accounts { get; set; }

        /// <summary>
        /// BANKMSGSRSV1, CREDITCARDMSGSRSV1
        /// </summary>
        public StatementResponse statement { get; set; }

        public int? ofx_response_status_code { get; set; }
        public string ofx_error_message { get; set; }

        public bool is_error
        {
            get { 
                return this.ofx_response_status_code.HasValue && !this.ofx_response_status_code.ToString().StartsWith("2"); 
            }
        }
    }
}
