using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.Service
{
    public class AccountTypeValues : ParameterValues
    {
		public AccountTypeValues()
            : base(string.Join(", ", Enum.GetNames(typeof(DTO.AccountType))))
        {
        }
    }
}
