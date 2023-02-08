﻿using CommunityToolkit.Mvvm.ComponentModel;
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
		[ObservableProperty]
		bool _LoginFailed = false;  //The storyboard thing.
		[ObservableProperty]
		string _Status = Loc.LoginEnterCred;
		[ObservableProperty]
		bool _UserErrorVisible = false;
		[ObservableProperty]
		bool _PassErrorVisible = false;

		#region IDataErrorInfo Members
		bool userNameError = true;
		bool passWordError = true;
		byte itCount = 0;   //To make the error invisible when first loaded(user didn't make any input).
		public string this[string data]
		{
			get
			{
				if (itCount <= 2) itCount++;
				string? errorMsg = string.Empty;
				switch (data)
				{
					case nameof(Username):
						if (Regex.IsMatch(Username, $"[^A-Z,a-z,0-9,_]") || Username.Length > 20 || string.IsNullOrWhiteSpace(Username))
						{
							userNameError = true;
							errorMsg = "Username error";
						}
						else
						{
							userNameError = false;
							errorMsg = null;
						}
						if (itCount > 2) UserErrorVisible = true;
						break;
					case nameof(Password):
						bool inValidFlag = false;
						foreach (char c in Password.ToCharArray())
							if (!char.IsAscii(c))
							{
								inValidFlag = true;
								break;
							}
						if (inValidFlag || (Password.Length >= 0 && Password.Length < 4))
						{
							passWordError = true;
							errorMsg = "Password error";
						}
						else
						{
							passWordError = false;
							errorMsg = null;
						}
						if (itCount > 2) PassErrorVisible = true;
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
				{   //Insert stuff into database.
					SqliteCommand cmd = connection.CreateCommand();
					cmd.CommandText = @"INSERT INTO Users (Username, Password, Type, Theme) VALUES ($Username, $Password, $Type, $Theme)";
					cmd.Parameters.AddWithValue("$Username", Username).SqliteType = SqliteType.Text;
					cmd.Parameters.AddWithValue("$Password", pwHash).SqliteType = SqliteType.Text;
					cmd.Parameters.AddWithValue("$Type", UserTypeEnum.User).SqliteType = SqliteType.Integer;
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
			{   //Register window closed by user.
			}
		}
		[RelayCommand]
		public void Login(Window loginWindow)
		{
			if (userNameError || passWordError) return;
			LoginFailed = false;
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
						LoginFailed = true;
						Status = Loc.LoginFailedStatus;
						connection.CloseAsync();
						return;
					}
					dataReader.Read();
					if (pwHash == dataReader.GetString(2))
					{   //PW correct.
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
						MainViewWindow mainView = new MainViewWindow(userInfo);
						mainView.Show();
						loginWindow.Close();
						return;
					}
					else
					{   //PW incorrect.
						LoginFailed = true; //UI picks this up and do some animations.
						Status = Loc.LoginFailedStatus;
					}
				}
				catch (SqliteException e)
				{
					MessageBox.Show(e.ToString(), "sth is wrong", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
				connection.CloseAsync();
			}
		}
	}
}
