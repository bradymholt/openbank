using System;

namespace OpenBank
{
	public class ServiceResponseBuilder
	{
		private OfxRequestor m_Requestor;
		public ServiceResponseBuilder (OfxRequestor requestor)
		{
			m_Requestor = requestor;
		}
	}
}

