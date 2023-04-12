using Microsoft.Data.Sqlite;
using System;
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
		public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();    //Global Logger initialize
		public App()
		{
			Logger.Info("The application is starting.");
			#region Load language settings.
			try
			{
				CultureInfo.CurrentUICulture = new CultureInfo(StdEqpTesting.Properties.Settings.Default.Locale);
			}
			catch (System.Exception ex)
			{
				Logger.Warn(ex.Message);
				Task.Run(() => MessageBox.Show($"{ex.Message}\nThe app will now use zh-CN as default language.", "Nope", MessageBoxButton.OK, MessageBoxImage.Asterisk));
				CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
			}
			Logger.Info("Current UI Culture is: " + CultureInfo.CurrentUICulture);
			#endregion
			#region Check db existance.
			Logger.Trace("Checking database");
			if (!File.Exists(StdEqpTesting.Properties.Settings.Default.DBConnString))
			{
				Logger.Warn("Database not found, initializing a new one.");
				//Must use '\' here since the settings is formatted like this.
				Directory.CreateDirectory(StdEqpTesting.Properties.Settings.Default.DBConnString.Remove(StdEqpTesting.Properties.Settings.Default.DBConnString.LastIndexOf('\\')));
				using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = StdEqpTesting.Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWriteCreate }.ToString()))
				{
					SqliteCommand comDataTableCmd = connection.CreateCommand();
					comDataTableCmd.CommandText = @"CREATE TABLE Users (
												ID	INTEGER NOT NULL UNIQUE,
												Username	TEXT NOT NULL UNIQUE,
												Password	TEXT NOT NULL,
												Type	INTEGER NOT NULL,
												Theme	INTEGER NOT NULL DEFAULT 0,
												Tag	TEXT,
												PRIMARY KEY(ID AUTOINCREMENT))";
					connection.Open();
					comDataTableCmd.ExecuteNonQuery();
				}
			}
			#endregion
#if RELEASE
			#region Global exception handling
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Dispatcher.UnhandledException += Dispatcher_UnhandledException;
			Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
			#endregion
#endif
			Current.Exit += Current_Exit;
		}
		#region Global exception handling
		private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			Logger.Fatal($"The sender is: {sender}\n{e.Exception}");
			MessageBox.Show($"The application ran into a serious problem and is going to be shutdown. The following information might be helpful:\nThe sender is: {sender}\n{e.Exception}\n\nAlso check the log folder.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
			throw new NotImplementedException();
		}

		public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion
		private void Current_Exit(object sender, ExitEventArgs e)
		{
			Logger.Info($"The application has stopped. [{e.ApplicationExitCode}]");
		}
	}
}