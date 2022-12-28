using StdEqpTesting.Model;
using StdEqpTesting.ViewModel;
using System.Windows;

namespace StdEqpTesting.View
{
	/// <summary>
	/// MainView.xaml 的交互逻辑
	/// </summary>
	public partial class MainView : Window
	{
		public static MainViewModel MainVM { get; } = new MainViewModel();
		public MainView(UserInfo userInfo)
		{
			InitializeComponent();
			DataContext = MainVM;
			Username.Content = userInfo.username;
			Type.Content = userInfo.type.ToString();
			if (userInfo.theme == 0)
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Dark.xaml");
			else if (userInfo.theme == 1)
				Resources.MergedDictionaries[0].Source = new System.Uri($"pack://application:,,,/Theme/Light.xaml");
		}
	}
}
