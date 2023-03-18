﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using StdEqpTesting.Localization;
using StdEqpTesting.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace StdEqpTesting.Model
{
	//It's more like a ViewModel now but whatever.
	public partial class TestTabItemModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<TestDataModel> _DataListBox = new ObservableCollection<TestDataModel>();
		[ObservableProperty]
		string _Message;    //Received String.
		[ObservableProperty]
		ObservableCollection<string> _UnitList = new ObservableCollection<string>();
		[ObservableProperty]
		bool _PortOpened = false;
		[ObservableProperty]
		decimal _Average = 0;

		//Props that doesn't need to be observed (not updating constantly on UI).
		public bool PropExpanded { get; set; } = false;
		public string AdditionalInfo { get; set; } = string.Empty;
		public bool AutoAdd { get; set; } = false;
		public bool AutoClear { get; set; } = true;
		public string TestName { get; set; } = string.Empty;
		public string MeaUnit { get; set; } = string.Empty;

		public bool IsUnitAdded = false;    //Used by Animation in code-behind.

		CancellationTokenSource cts;
		CancellationToken ct;
		SerialPort serialPort = new SerialPort();
		System.Timers.Timer timer = new System.Timers.Timer(1000) { AutoReset = true };
		[RelayCommand]
		public void Connect()
		{
			if (PortOpened)
			{
				timer.Close();
				cts.Cancel();
				serialPort.Close();
				cts.Dispose();
				MainViewModel.MainVM.LogSth($"Serial port {PortName} closed.", 2);
			}
			else
			{
				cts = new CancellationTokenSource();
				ct = cts.Token;

				serialPort.PortName = PortName;
				serialPort.BaudRate = BaudRate;
				serialPort.Parity = Parity;
				serialPort.DataBits = DataBits;
				serialPort.StopBits = StopBits;
				serialPort.Handshake = Handshake;
				serialPort.Encoding = Encoding;

				serialPort.ReadTimeout = ReadTimeout;
				serialPort.WriteTimeout = WriteTimeout;

				try
				{
					serialPort.Open();
					Task.Run(() =>
					{   //This task will keep running while the port is open.
						while (true)
						{
							ct.ThrowIfCancellationRequested();  //This will throw an exception, which is not handled, thus stopping the task.
							string incoming = serialPort.ReadExisting();
							if (!string.IsNullOrEmpty(incoming))
							{
								Message += incoming;
								timer.Interval = 1000;  //Setting the interval will cause the timer to reset.
							}
							Thread.Sleep(160);  //Wait time before next serial read to save some CPU time.
						}
					}, ct);
					MainViewModel.MainVM.LogSth($"Serial port {PortName} opened.", 2);
					timer.Start();
				}
				catch (Exception e)
				{   //TODO: Make it pretty.
					MainViewModel.MainVM.LogSth(e.ToString(), 4);
					MessageBox.Show(e.Message);
				}
			}
			PortOpened = serialPort.IsOpen;
		}

		private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{   //Timer elapsed, add value to list and restart timer.
			if (AutoAdd) Add2List();
		}

		[RelayCommand]
		public void Add2List()
		{
			if (!string.IsNullOrWhiteSpace(Message))
			{
				DataListBox.Add(new TestDataModel { Index = DataListBox.Count + 1, Value = Message });
				if (AutoClear) Message = string.Empty;
			}
		}
		[RelayCommand]
		public async void SaveValue(TestDataModel? value)
		{
			if ((string.IsNullOrWhiteSpace(TestName) || string.IsNullOrWhiteSpace(MeaUnit)) && MessageBox.Show(Loc.ConfirmAddEmptyRecordDesc, Loc.ConfirmAddEmptyRecord, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
				return;
			MainViewModel.MainVM.UpdateMainStatus(Loc.DBConnect, true);
			await Task.Run(() =>
			{
				using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
				{   //Check if ComTestData exists or not.
					SqliteCommand command = connection.CreateCommand();
					command.CommandText = @"SELECT EXISTS (
											SELECT name
											FROM sqlite_schema 
											WHERE type='table' AND name='ComTestData')";
					try
					{
						connection.Open();
						SqliteDataReader reader = command.ExecuteReader();
						reader.Read();
						if (int.Parse(reader.GetString(0)) == 0)    //Does not exist
						{   //If not exist, create table.
							reader.Close();
							App.Logger.Warn("ComTestData table does not exist, creating a new one.");
							command.CommandText = @"CREATE TABLE ComTestData (
													ID	INTEGER NOT NULL UNIQUE,
													User	TEXT NOT NULL,
													TestName	TEXT NOT NULL,
													ValueType	TEXT,
													TestValue	TEXT NOT NULL,
													TestUnit	TEXT NOT NULL,
													Tag	TEXT,
													COMPort	TEXT,
													Time	INTEGER NOT NULL,
													FOREIGN KEY(User) REFERENCES Users(Username),
													PRIMARY KEY(ID AUTOINCREMENT))";
							command.ExecuteNonQuery();
						}
						else reader.Close();
						//Now it exists
						command.CommandText = @"INSERT INTO ComTestData (User, TestName, ValueType, TestValue, TestUnit, Tag, COMPort, Time)
												VALUES ($User, $TestName, $ValueType, $TestValue, $TestUnit, $Tag, $COMPort, $Time)";
						command.Parameters.AddWithValue("$User", MainViewModel.MainVM.HomeViewVM.UserName);
						command.Parameters.AddWithValue("$TestName", TestName);
						command.Parameters.AddWithValue("$ValueType", value is null ? "Average" : "Selected");
						command.Parameters.AddWithValue("$TestValue", value is null ? Average.ToString() : value.Value);
						command.Parameters.AddWithValue("$TestUnit", MeaUnit);
						command.Parameters.AddWithValue("$Tag", AdditionalInfo);
						command.Parameters.AddWithValue("$COMPort", PortName);
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
		}

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
				File.WriteAllLines($"Database{Path.DirectorySeparatorChar}Units.txt", UnitList);
				MainViewModel.MainVM.UpdateSecStatus(Loc.AddUnitSucc, true);
			}
		}

		public bool NoCOM { get; }

		#region Port properties
		public string PortName { get; set; }
		public int BaudRate { get; set; } = 9600;
		public Parity Parity { get; set; } = Parity.None;
		public int DataBits { get; set; } = 8;
		public StopBits StopBits { get; set; } = StopBits.One;
		public Handshake Handshake { get; set; } = Handshake.None;
		public Encoding Encoding { get; set; } = Encoding.UTF8;
		public int ReadTimeout { get; set; } = 1000;
		public int WriteTimeout { get; set; } = 1000;
		#endregion

		private void DataListBox_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
				for (ushort i = 0; i < DataListBox.Count;)
					DataListBox[i].Index = ++i;
			Task.Run(() =>
			{
				decimal[] validValue = (from data in DataListBox
										where decimal.TryParse(data.Value, out _) == true
										select decimal.Parse(data.Value)).ToArray();
				Average = validValue.Length == 0 ? 0 : validValue.Average();
			});
		}

		public TestTabItemModel(bool hasCOM = true)
		{
			NoCOM = !hasCOM;
			timer.Elapsed += Timer_Elapsed;
			DataListBox.CollectionChanged += DataListBox_CollectionChanged;
			//Read units from file.
			if (File.Exists($"Database{Path.DirectorySeparatorChar}Units.txt"))
			{
				string[] unitsInFile = File.ReadAllLines($"Database{Path.DirectorySeparatorChar}Units.txt");
				_UnitList.Clear();
				foreach (string unit in unitsInFile)
					UnitList.Add(unit);
			}
			BindingOperations.EnableCollectionSynchronization(DataListBox, this);
		}
	}
}
