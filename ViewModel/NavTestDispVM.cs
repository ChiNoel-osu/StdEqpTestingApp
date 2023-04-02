using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using StdEqpTesting.Localization;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StdEqpTesting.ViewModel
{
	public partial class NavTestDispVM : ObservableObject
	{
		#region InputDataHandling
		[ObservableProperty]
		decimal _Displacement = 0;
		decimal _PointA = 0;
		public decimal PointA
		{
			get => _PointA;
			set
			{
				_PointA = value;
				Displacement = Math.Abs(_PointA - _PointB);
			}
		}
		decimal _PointB = 0;
		public decimal PointB
		{
			get => _PointB;
			set
			{
				_PointB = value;
				Displacement = Math.Abs(_PointA - _PointB);
			}
		}
		#endregion

		[ObservableProperty]
		ObservableCollection<string> _UnitList = new ObservableCollection<string>();
		public string TestName { get; set; } = string.Empty;
		public string MeaUnit { get; set; } = string.Empty;
		public string AdditionalInfo { get; set; } = string.Empty;
		public bool IsUnitAdded = false;    //Used by Animation in code-behind.
		public void AddUnit()
		{
			if (UnitList.Contains(MeaUnit))
			{   //Unit already added.
				IsUnitAdded = false;
				MainViewModel.MainVM.UpdateSecStatus(Loc.AddUnitFail_Existed, true, 3);
			}
			else if (string.IsNullOrWhiteSpace(MeaUnit))
			{   //Unit invalid.
				IsUnitAdded = false;
				MainViewModel.MainVM.UpdateSecStatus(Loc.AddUnitFail_Empty, true, 3);
			}
			else
			{
				IsUnitAdded = true;
				UnitList.Add(MeaUnit);
				File.WriteAllLines($"Database{Path.DirectorySeparatorChar}UnitsDISP.txt", UnitList);
				MainViewModel.MainVM.UpdateSecStatus(Loc.AddUnitSucc, true);
			}
		}

		[RelayCommand]
		public async void SaveDispValue()
		{
			if ((string.IsNullOrWhiteSpace(TestName) || string.IsNullOrWhiteSpace(MeaUnit)) && MessageBox.Show(Loc.ConfirmAddEmptyRecordDesc, Loc.ConfirmAddEmptyRecord, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
				return;
			Mouse.OverrideCursor = Cursors.AppStarting;
			App.Logger.Info("Saving displacement value to DB.");
			MainViewModel.MainVM.UpdateMainStatus(Loc.DBConnect, true);
			await Task.Run(() =>
			{
				using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
				{   //Check if DispTestData exists or not.
					SqliteCommand command = connection.CreateCommand();
					command.CommandText = @"SELECT EXISTS (
											SELECT name
											FROM sqlite_schema 
											WHERE type='table' AND name='DispTestData')";
					try
					{
						connection.Open();
						SqliteDataReader reader = command.ExecuteReader();
						reader.Read();
						if (int.Parse(reader.GetString(0)) == 0)    //Does not exist
						{   //If not exist, create table.
							reader.Close();
							App.Logger.Warn("DispTestData table does not exist, creating a new one.");
							command.CommandText = @"CREATE TABLE DispTestData (
													ID	INTEGER NOT NULL UNIQUE,
													User	TEXT NOT NULL,
													TestName	TEXT NOT NULL,
													TestValue	INTEGER NOT NULL,
													TestUnit	TEXT NOT NULL,
													Tag	TEXT,
													Time	INTEGER NOT NULL,
													PRIMARY KEY(ID AUTOINCREMENT),
													FOREIGN KEY(User) REFERENCES Users(Username))";
							command.ExecuteNonQuery();
						}
						else reader.Close();
						//Now it exists
						command.CommandText = @"INSERT INTO DispTestData (User, TestName, TestValue, TestUnit, Tag, Time)
												VALUES ($User, $TestName, $TestValue, $TestUnit, $Tag, $Time)";
						command.Parameters.AddWithValue("$User", MainViewModel.MainVM.HomeViewVM.UserName);
						command.Parameters.AddWithValue("$TestName", TestName);
						command.Parameters.AddWithValue("$TestValue", Displacement);
						command.Parameters.AddWithValue("$TestUnit", MeaUnit);
						command.Parameters.AddWithValue("$Tag", AdditionalInfo);
						command.Parameters.AddWithValue("$Time", DateTimeOffset.Now.ToUnixTimeSeconds());   //Time are stored as Unix Timestamp.
						command.ExecuteNonQuery();
						MainViewModel.MainVM.UpdateMainStatus(Loc.Saved2DB, true);
					}
					catch (SqliteException e)
					{
						MainViewModel.MainVM.UpdateMainStatus(Loc.NotSaved2DB, true, 4);
						App.Logger.Error("Something is wrong when saving value.\n" + e.ToString());
						MessageBox.Show(e.Message, "sth is wrong", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				};
			});
			Mouse.OverrideCursor = null;
		}

		public NavTestDispVM()
		{
			//Read units from file.
			if (File.Exists($"Database{Path.DirectorySeparatorChar}UnitsDISP.txt"))
			{
				string[] unitsInFile = File.ReadAllLines($"Database{Path.DirectorySeparatorChar}UnitsDISP.txt");
				_UnitList.Clear();
				foreach (string unit in unitsInFile)
					UnitList.Add(unit);
				App.Logger.Info($"Read {unitsInFile.Length} displacement units.");
			}
		}
	}
}
