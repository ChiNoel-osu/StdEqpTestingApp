using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using StdEqpTesting.Localization;
using StdEqpTesting.Model;
using StdEqpTesting.View;
using System;
using System.ComponentModel;
using System.Security.Cryptography;
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
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte hash in SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(Password)))
					stringBuilder.Append(hash.ToString("X2"));
				string pwHash = stringBuilder.ToString();
				using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
				{
					SqliteCommand cmd = connection.CreateCommand();
					cmd.CommandText = @"INSERT INTO Users (Username, Password, Type) VALUES ($Username, $Password, $Type)";
					cmd.Parameters.AddWithValue("$Username", Username).SqliteType = SqliteType.Text;
					cmd.Parameters.AddWithValue("$Password", pwHash).SqliteType = SqliteType.Text;
					cmd.Parameters.AddWithValue("Type", UserTypeEnum.Owner).SqliteType = SqliteType.Integer;
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
				}
			}
			else
			{
			}
		}
	}
}
