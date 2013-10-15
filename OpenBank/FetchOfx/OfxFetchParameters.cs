using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.FetchOfx
{
    public class OfxFetchParameters
    {
		[ParameterRequired]
        public string OFX_URL { get; set; }

		[ParameterRequired]
        public string FID { get; set; }

		[ParameterRequired]
        public string ORG { get; set; }

		[ParameterRequired]
        public string UserID { get; set; }

		[ParameterRequired]
        public string Password { get; set; }
    }
}
