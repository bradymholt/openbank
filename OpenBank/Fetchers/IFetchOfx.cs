using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfxClient
{
    public interface IFetchOfx
    {
        OfxAccount FetchOfx();
        string Request { get; }
        string Response { get; }
    }
}
