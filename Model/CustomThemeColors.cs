using System.Windows.Media;

namespace StdEqpTesting.Model
{
	public struct CustomThemeColors
	{
		public readonly struct DarkTheme
		{
			public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Color.FromRgb(30, 30, 30));
			public static readonly SolidColorBrush ForegroundBrush = new SolidColorBrush(Colors.White);
			public static readonly SolidColorBrush CaretBrush = new SolidColorBrush(Colors.WhiteSmoke);
		}
		public readonly struct LightTheme
		{
			public static readonly SolidColorBrush BackgroundBrush = new SolidColorBrush(Colors.White);
			public static readonly SolidColorBrush ForegroundBrush = new SolidColorBrush(Colors.Black);
			public static readonly SolidColorBrush CaretBrush = new SolidColorBrush(Colors.Black);
		}
	}
}
