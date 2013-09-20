using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBank.Service
{
    public class ParameterFormat : Attribute
    {
        public ParameterFormat() { }

        public ParameterFormat(string format)
            :this()
        {
            this.Format = format;
        }

        public string Format { get; set; }
    }
}
