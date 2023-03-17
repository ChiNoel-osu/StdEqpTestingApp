using System;
using System.Globalization;
using System.Windows.Data;

namespace StdEqpTesting.Converter
{
	public class IsObjectNullConverter : IValueConverter
	{
		/// <summary>
		/// Convert an Object's null-ness to a bool value
		/// </summary>
		/// <param name="value">The object to evaluate</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">Is the result inverted?</param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return bool.Parse((string)parameter) ? value is not null : value is null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
