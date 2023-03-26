using System.Windows;
using System.Windows.Controls;

namespace StdEqpTesting.View
{
	/// <summary>
	/// NavReview.xaml 的交互逻辑
	/// </summary>
	public partial class NavReview : UserControl
	{
		public NavReview()
		{
			InitializeComponent();
		}

		private void DataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Delete && MessageBox.Show(Localization.Loc.ReviewConfirmDelete, string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
				e.Handled = true;
		}
	}
}
