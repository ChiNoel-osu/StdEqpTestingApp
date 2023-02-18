using System.Windows;
using System.Windows.Controls;

namespace StdEqpTesting.View
{
	/// <summary>
	/// LoginWindow.xaml 的交互逻辑
	/// </summary>
	public partial class LoginWindow : Window
	{
		public LoginWindow()
		{
			App.Logger.Info("Initializing Login Window.");
			InitializeComponent();
			DataContext = MainWindow.MainVM;
		}
		private void ThemeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (((Button)sender).Content.ToString() == "🌙")
			{
				((Button)sender).Content = "☀";
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Dark.xaml");
			}
			else if (((Button)sender).Content.ToString() == "☀")
			{
				((Button)sender).Content = "🌙";
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Light.xaml");
			}
		}
	}
}
