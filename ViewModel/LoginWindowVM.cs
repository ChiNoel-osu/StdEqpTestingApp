using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using StdEqpTesting.Localization;
using StdEqpTesting.Model;
using StdEqpTesting.View;
using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace StdEqpTesting.ViewModel
{
	public partial class LoginWindowVM : ObservableObject, IDataErrorInfo
	{
		public string Username { get; set; }
		public string Password { get; set; }
		[ObservableProperty]
		bool _NoInputError = false; //Button IsEnabled

		#region IDataErrorInfo Members
		bool userNameError = false;
		bool passWordError = false;
		public string this[string data]
		{
			get
			{
				string? errorMsg = string.Empty;
				switch (data)
				{
					case nameof(Username):
						if (Regex.IsMatch(Username, $"[^A-Z,a-z,0-9,_]") || Username.Length > 20)
						{
							userNameError = true;
							errorMsg = "Username error";
						}
						else
						{
							userNameError = false;
							errorMsg = null;
						}
						break;
					case nameof(Password):
						bool inValidFlag = false;
						foreach (char c in Password.ToCharArray())
							if (!char.IsAscii(c))
							{
								inValidFlag = true;
								break;
							}
						if (inValidFlag || (Password.Length > 0 && Password.Length < 4))
						{
							passWordError = true;
							errorMsg = "Password error";
						}
						else
						{
							passWordError = false;
							errorMsg = null;
						}
						break;
					default:
						throw new NotImplementedException();
				}
				if (Username.Length == 0 || Password.Length == 0 || passWordError || userNameError) NoInputError = false;
				else NoInputError = true;
				return errorMsg;
			}
		}
		public string Error => throw new System.NotImplementedException();
		#endregion

		[RelayCommand]
		public void Register(string currentTheme)
		{
			if (currentTheme == "🌙")
				currentTheme = "Light";
			else if (currentTheme == "☀")
				currentTheme = "Dark";
			RegisterWindow rWnd = new RegisterWindow(Username, Password, currentTheme);
			if ((bool)rWnd.ShowDialog())
			{   //Register confirmed
				//Hash the password to store in DB.
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte hash in SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(Password)))
					stringBuilder.Append(hash.ToString("X2"));
				string pwHash = stringBuilder.ToString();
				using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
				{
					SqliteCommand cmd = connection.CreateCommand();
					cmd.CommandText = @"INSERT INTO Users (Username, Password, Type, Theme) VALUES ($Username, $Password, $Type, $Theme)";
					cmd.Parameters.AddWithValue("$Username", Username).SqliteType = SqliteType.Text;
					cmd.Parameters.AddWithValue("$Password", pwHash).SqliteType = SqliteType.Text;
					cmd.Parameters.AddWithValue("$Type", UserTypeEnum.Owner).SqliteType = SqliteType.Integer;
					if (currentTheme == "Dark")
						cmd.Parameters.AddWithValue("$Theme", 0).SqliteType = SqliteType.Integer;
					else if (currentTheme == "Light")
						cmd.Parameters.AddWithValue("$Theme", 1).SqliteType = SqliteType.Integer;
					else
						throw new NotImplementedException();
					connection.Open();
					try
					{
						cmd.ExecuteNonQuery();
						MessageBox.Show(Loc.RegisterDoneMsgBoxContent, Loc.RegisterDoneMsgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (SqliteException e)
					{
						MessageBox.Show(e.ToString(), Loc.RegisterFailMsgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
					connection.CloseAsync();
				}
			}
			else
			{   //Register window closed
			}
		}
		[RelayCommand]
		public void Login(Window loginWindow)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte hash in SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(Password)))
				stringBuilder.Append(hash.ToString("X2"));
			string pwHash = stringBuilder.ToString();
			using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
			{
				SqliteDataReader dataReader;
				SqliteCommand cmd = connection.CreateCommand();
				cmd.CommandText = @"SELECT * FROM Users WHERE Username=$Username LIMIT 1";
				cmd.Parameters.AddWithValue("$Username", Username).SqliteType = SqliteType.Text;
				connection.Open();
				try
				{
					dataReader = cmd.ExecuteReader();
					if (!dataReader.HasRows)
					{
						MessageBox.Show("Nope");
						connection.CloseAsync();
						return;
					}
					dataReader.Read();
					if (pwHash == dataReader.GetString(2))
					{	//PW correct.
						UserInfo userInfo = new UserInfo()
						{
							ID = dataReader.GetInt32(0),
							username = dataReader.GetString(1),
							//password = dataReader.GetString(2),	//Gets the hashed pw, not needed.
							type = (UserTypeEnum)dataReader.GetInt32(3),
							theme = dataReader.GetInt32(4),
							tag = dataReader.IsDBNull(5) ? null : dataReader.GetString(5)
						};
						connection.CloseAsync();
						//Starts main view.
						MainView mainView = new MainView(userInfo);
						mainView.Show();
						loginWindow.Close();
						return;
					}
					else
					{	//PW incorrect.
						MessageBox.Show("Wrong number");
					}
				}
				catch (SqliteException e)
				{
					MessageBox.Show(e.ToString(), "FUCK", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
				connection.CloseAsync();
			}
		}
	}
}
