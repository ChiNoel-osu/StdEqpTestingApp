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
		readonly Uri darkUri = new Uri($"pack://application:,,,/Theme/Dark.xaml");
		readonly Uri lightUri = new Uri($"pack://application:,,,/Theme/Light.xaml");
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
		#region TabAnimations	//Code behind solution enables dynamic resource.
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
		public HomeViewWindow(UserInfo userInfo)
		{
			InitializeComponent();
			DataContext = MainViewModel.MainVM;
			//Gets the user info passed from loginWindow.
			((MainViewModel)DataContext).HomeViewVM.UserName = userInfo.username;
			((MainViewModel)DataContext).HomeViewVM.UserType = userInfo.type;
			((MainViewModel)DataContext).SecondaryStatus = Localization.Loc.StatusLoggedIn.Replace("%User", userInfo.username).Replace("%Type", userInfo.type.ToString());
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
			this.Closed += HomeWindow_Closed;
		}

		private void HomeWindow_Closed(object? sender, EventArgs e)
		{
			//Close the video capture device.
			((MainViewModel)DataContext).NavTestImgVM.VCD?.SignalToStop();
		}
	}
}
