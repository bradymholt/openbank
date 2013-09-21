using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenBank.Service
{
    public class ParameterHelper
    {
        public static DateTime GetDateParameter(string dateText)
        {
            DateTime date;
			if (string.IsNullOrEmpty(dateText) || !DateTime.TryParseExact(dateText, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                date = DateTime.Now;
            }

            return date;

        }
    }
}
