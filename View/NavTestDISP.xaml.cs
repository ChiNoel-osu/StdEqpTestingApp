using StdEqpTesting.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StdEqpTesting.View
{
	/// <summary>
	/// NavTestDISP.xaml 的交互逻辑
	/// </summary>
	public partial class NavTestDISP : UserControl
	{
		public NavTestDISP()
		{
			InitializeComponent();
		}
		#region AddUnitAnimation
		readonly ColorAnimation SuccAni = new ColorAnimation()
		{
			From = Colors.Lime,
			Duration = TimeSpan.FromSeconds(2),
			EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn },
		};
		readonly ColorAnimation FailAni = new ColorAnimation()
		{
			From = Colors.Red,
			Duration = TimeSpan.FromSeconds(2),
			EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn },
		};
		Storyboard storyboard = new Storyboard();
		private void AddUnit_Button_Click(object sender, RoutedEventArgs e)
		{
			NavTestDispVM dContext = ((MainViewModel)((Button)sender).DataContext).NavTestDispVM;
			dContext.AddUnit();
			if (dContext.IsUnitAdded)
			{
				SuccAni.To = ((SolidColorBrush)FindResource("GlobalFG")).Color;
				Storyboard.SetTargetProperty(SuccAni, new PropertyPath("(Button.Foreground).(SolidColorBrush.Color)", null));
				storyboard.Children.Add(SuccAni);
				storyboard.Begin((Button)sender);
				storyboard.Children.Clear();
			}
			else
			{
				FailAni.To = ((SolidColorBrush)FindResource("GlobalFG")).Color;
				Storyboard.SetTargetProperty(FailAni, new PropertyPath("(Button.Foreground).(SolidColorBrush.Color)", null));
				storyboard.Children.Add(FailAni);
				storyboard.Begin((Button)sender);
				storyboard.Children.Clear();
			}
		}
		#endregion
	}
}
