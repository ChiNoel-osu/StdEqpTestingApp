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
#if DEBUG
			//Continue as normal.
#endif
#if RELEASE
			MainVM.MainWindowVM.LoadNextWnd(this);
#endif
#if SKIPLOGINDBG
			View.HomeViewWindow mainViewWindow = new View.HomeViewWindow(new Model.UserInfo { ID = 0, theme = 0, type = (Model.UserTypeEnum)0, username = "VicTest" });
			mainViewWindow.Show();
			Close();
#endif
		}
	}
}