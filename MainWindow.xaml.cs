using StdEqpTesting.ViewModel;
using System.Windows;

namespace StdEqpTesting
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainViewModel MainVM { get; } = new MainViewModel();
		public MainWindow()
		{
			InitializeComponent();
			DataContext = MainVM;
		}
	}
}
