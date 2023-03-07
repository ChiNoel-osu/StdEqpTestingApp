using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StdEqpTesting.Converter
{
	public class InvertedBool2VisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value switch
			{
				Visibility.Collapsed => true,
				Visibility.Visible => (object)false,
				_ => throw new NotImplementedException(),
			};
		}
	}
}