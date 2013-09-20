using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.OfxAssimilate
{
    public class OfxFetchParameters
    {
        public string OFX_URL { get; set; }
        public string FID { get; set; }
        public string ORG { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
    }
}
