using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class OfxRequest
    {
        public string Fid { get; set; }
        public string Org { get; set; }
        public string Url { get; set; }
        public string BankID { get; set; }
        public string UserID { get; set; }
        public string UserPassword { get; set; }
        public string AccountID { get; set; }
        public OfxAccountType AccountType { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

    }
}
