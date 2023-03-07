using StdEqpTesting.Model;
using StdEqpTesting.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StdEqpTesting.View
{
	/// <summary>
	/// HomeViewWindow.xaml 的交互逻辑
	/// </summary>
	public partial class HomeViewWindow : Window
	{
		public static MainViewModel MainVM { get; } = new MainViewModel();
		readonly Uri darkUri = new Uri($"pack://application:,,,/Theme/Dark.xaml");
		readonly Uri lightUri = new Uri($"pack://application:,,,/Theme/Light.xaml");
		public HomeViewWindow(UserInfo userInfo)
		{
			InitializeComponent();
			DataContext = MainVM;
			//Gets the user info passed from loginWindow.
			Username.Content = userInfo.username;
			Type.Content = userInfo.type.ToString();
			#region Theme Init
			if (userInfo.theme == 0)
			{
				ThemeBtn.Content = "☀";
				Resources.MergedDictionaries[0].Source = darkUri;
			}
			else if (userInfo.theme == 1)
			{
				ThemeBtn.Content = "🌙";
				Resources.MergedDictionaries[0].Source = lightUri;
			}
			#endregion
		}
		private void ThemeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (((Button)sender).Content.ToString() == "🌙")
			{
				((Button)sender).Content = "☀";
				Resources.MergedDictionaries[0].Source = darkUri;
			}
			else if (((Button)sender).Content.ToString() == "☀")
			{
				((Button)sender).Content = "🌙";
				Resources.MergedDictionaries[0].Source = lightUri;
			}
		}
		#region TavAnimations	//Code behind solution enables dynamic resource.
		object TavAnimations = new object();
		readonly ColorAnimation tabBGAni = new ColorAnimation() { Duration = TimeSpan.FromMilliseconds(200) };
		readonly ColorAnimation tabFGAni = new ColorAnimation() { Duration = TimeSpan.FromMilliseconds(200) };
		Storyboard storyboard = new Storyboard();
		private void TabChecked(object sender, RoutedEventArgs e)
		{
			tabBGAni.To = ((SolidColorBrush)FindResource("CheckedTabBG")).Color;
			tabFGAni.To = ((SolidColorBrush)FindResource("GlobalBG")).Color;
			Storyboard.SetTargetProperty(tabBGAni, new PropertyPath("(RadioButton.Background).(SolidColorBrush.Color)", null));
			Storyboard.SetTargetProperty(tabFGAni, new PropertyPath("(RadioButton.Foreground).(SolidColorBrush.Color)", null));
			storyboard.Children.Add(tabBGAni);
			storyboard.Children.Add(tabFGAni);
			storyboard.Begin((RadioButton)sender);
			storyboard.Children.Clear();
		}
		private void TabUnchecked(object sender, RoutedEventArgs e)
		{
			tabBGAni.To = Colors.Transparent;
			tabFGAni.To = ((SolidColorBrush)FindResource("GlobalFG")).Color;
			Storyboard.SetTargetProperty(tabBGAni, new PropertyPath("(RadioButton.Background).(SolidColorBrush.Color)", null));
			Storyboard.SetTargetProperty(tabFGAni, new PropertyPath("(RadioButton.Foreground).(SolidColorBrush.Color)", null));
			storyboard.Children.Add(tabBGAni);
			storyboard.Children.Add(tabFGAni);
			storyboard.Begin((RadioButton)sender);
			storyboard.Children.Clear();
		}
		#endregion
	}
}
