﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.DTO
{
    /// <summary>
    /// ACCTTYPE
    /// </summary>
    public enum AccountType : int
    {
        CHECKING,
        SAVING,
        MONEYMRKT,
        CREDITCARD,
        OTHER
    }
}
