using System.Windows.Media;

namespace StdEqpTesting.Model
{
	public static class CustomThemeColors
	{
		public static class DarkTheme
		{
			public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromRgb(30,30,30));
			public static readonly SolidColorBrush ForegroundBrush = new SolidColorBrush(Colors.White);
			public static readonly SolidColorBrush CaretBrush = new SolidColorBrush(Colors.WhiteSmoke);
		}
		public static class LightTheme
		{
			public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Colors.White);
			public static readonly SolidColorBrush ForegroundBrush = new SolidColorBrush(Colors.Black);
			public static readonly SolidColorBrush CaretBrush = new SolidColorBrush(Colors.Black);
		}
	}
}
