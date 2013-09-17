using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank
{
    public class ParameterFormat : Attribute
    {
        public ParameterFormat(string format)
        {
            this.Format = format;
        }

        public string Format { get; set; }
    }
}
