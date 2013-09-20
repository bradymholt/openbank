using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.OfxData
{
    /// <summary>
    /// ACCTTYPE
    /// </summary>
    public enum OfxAccountType : int
    {
        CHECKING,
        SAVING,
        MONEYMRKT,
        CREDITCARD,
        OTHER
    }
}
