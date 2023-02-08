using Microsoft.Data.Sqlite;
using System.Globalization;
using System.IO;
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
			#region Check db existance.
			if (!File.Exists(StdEqpTesting.Properties.Settings.Default.DBConnString))
			{
				Directory.CreateDirectory(StdEqpTesting.Properties.Settings.Default.DBConnString.Remove(StdEqpTesting.Properties.Settings.Default.DBConnString.LastIndexOf('\\')));
				using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = StdEqpTesting.Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWriteCreate }.ToString()))
				{
					SqliteCommand sqliteCommand = connection.CreateCommand();
					sqliteCommand.CommandText = @"CREATE TABLE Users (
												ID	INTEGER NOT NULL UNIQUE,
												Username	TEXT NOT NULL UNIQUE,
												Password	TEXT NOT NULL,
												Type	INTEGER NOT NULL,
												Theme	INTEGER NOT NULL DEFAULT 0,
												Tag	TEXT,
												PRIMARY KEY(ID AUTOINCREMENT))";
					connection.Open();
					sqliteCommand.ExecuteNonQuery();
				}
			}
			#endregion
		}
	}
}
