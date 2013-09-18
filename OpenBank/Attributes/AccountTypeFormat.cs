using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class AccountTypeFormat : ParameterFormat
    {
        public AccountTypeFormat()
            : base(string.Join(", ", Enum.GetNames(typeof(AccountType))))
        {
        }
    }
}
