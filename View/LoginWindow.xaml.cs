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
			InitializeComponent();
			DataContext = MainWindow.MainVM;
		}
		string currentTheme = "Dark";
		private void ThemeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (((Button)sender).Content.ToString() == "🌙")
			{
				((Button)sender).Content = "☀";
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Dark.xaml");
				currentTheme = "Dark";
			}
			else if (((Button)sender).Content.ToString() == "☀")
			{
				((Button)sender).Content = "🌙";
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Light.xaml");
				currentTheme = "Light";
			}
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{   //You need to use Code-behind to get PasswordBox's password as it can not be databound.
			//Probably not safe but whatever.
			RegisterWindow rWnd = new RegisterWindow(Username.Text, Password.Password, currentTheme);
			if ((bool)rWnd.ShowDialog())
			{
				Password.Clear();
			}
			else
			{
			}
		}
	}
}
