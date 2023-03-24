using StdEqpTesting.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StdEqpTesting.View
{
	/// <summary>
	/// NavSettings.xaml 的交互逻辑
	/// </summary>
	public partial class NavSettings : UserControl
	{
		public NavSettings()
		{
			InitializeComponent();
		}

		#region COMSaveAnimation
		readonly ColorAnimation SuccAni = new ColorAnimation()
		{
			From = Colors.Lime,
			Duration = TimeSpan.FromSeconds(2),
			EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn },
		};
		Storyboard storyboard = new Storyboard();
		private void SaveCOMButton_Click(object sender, RoutedEventArgs e)
		{
			MainViewModel dContext = (MainViewModel)((Button)sender).DataContext;
			dContext.NavSettingsVM.SaveCOMSettings(COMSettingPort.SelectedItem.ToString());
			if (dContext.NavSettingsVM.saved)
			{
				SuccAni.To = ((SolidColorBrush)FindResource("GlobalFG")).Color;
				Storyboard.SetTargetProperty(SuccAni, new PropertyPath("(Button.Foreground).(SolidColorBrush.Color)", null));
				storyboard.Children.Add(SuccAni);
				storyboard.Begin((Button)sender);
				storyboard.Children.Clear();
			}
		}
		#endregion
	}
}
