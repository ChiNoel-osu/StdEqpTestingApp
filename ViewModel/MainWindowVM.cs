using CommunityToolkit.Mvvm.Input;
using StdEqpTesting.View;
using System.Windows;

namespace StdEqpTesting.ViewModel
{
	public partial class MainWindowVM
	{
		[RelayCommand]
		public void LoadNextWnd(Window parent)
		{
			LoginWindow loginWindow = new LoginWindow();
			parent.Close();
			loginWindow.Show();
		}
	}
}
