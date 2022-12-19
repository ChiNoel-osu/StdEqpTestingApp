using System.Windows;
using System.Windows.Controls;

namespace StdEqpTesting.View
{
	/// <summary>
	/// RegisterWindow.xaml 的交互逻辑
	/// </summary>
	public partial class RegisterWindow : Window
	{
		readonly string username, password;

		public RegisterWindow(string username, string password, string theme)
		{
			this.username = username;
			this.password = password;
			InitializeComponent();
			if (theme == "Dark")
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Dark.xaml");
			else if (theme == "Light")
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Light.xaml");
			RegName.Content = username;
		}

		private void Password_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (((PasswordBox)sender).Password != password)
				RegButton.IsEnabled = false;
			else
				RegButton.IsEnabled = true;
		}

		private void RegButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
