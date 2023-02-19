using StdEqpTesting.Model;
using StdEqpTesting.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace StdEqpTesting.View
{
	/// <summary>
	/// HomeViewWindow.xaml 的交互逻辑
	/// </summary>
	public partial class HomeViewWindow : Window
	{
		public static MainViewModel MainVM { get; } = new MainViewModel();
		public HomeViewWindow(UserInfo userInfo)
		{
			InitializeComponent();
			DataContext = MainVM;
			Username.Content = userInfo.username;
			Type.Content = userInfo.type.ToString();
			if (userInfo.theme == 0)
			{
				ThemeBtn.Content = "☀";
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Dark.xaml");
			}
			else if (userInfo.theme == 1)
			{
				ThemeBtn.Content = "🌙";
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Light.xaml");
			}
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
