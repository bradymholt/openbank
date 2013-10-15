using System;
using System.Globalization;

namespace OpenBank
{
	public static class ParameterHelper
	{
		public static DateTime ParseDateParameter(string dateText)
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

