using StdEqpTesting.View;
using StdEqpTesting.ViewModel;
using System.Windows;

namespace StdEqpTesting
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static StartupMainViewModel MainVM { get; } = new StartupMainViewModel();
		public MainWindow()
		{
			InitializeComponent();
			DataContext = MainVM;

			MainViewWindow mainViewWindow = new MainViewWindow(new Model.UserInfo { ID = 0, theme = 0, type = 0, username = "VicTest" });
			mainViewWindow.Show();
			Close();    //For testing. Closes the window right after running code MainWindowVM
		}
	}
}
