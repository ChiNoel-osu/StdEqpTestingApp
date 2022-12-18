using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace StdEqpTesting
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			#region Load settings.
			try
			{
				CultureInfo.CurrentUICulture = new CultureInfo(StdEqpTesting.Properties.Settings.Default.Locale);
			}
			catch (System.Exception ex)
			{
				Task.Run(() => MessageBox.Show($"{ex.Message}\nThe app will now use zh-CN as default language.", "Nope", MessageBoxButton.OK, MessageBoxImage.Asterisk));
				CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
			}
			#endregion
		}
	}
}
