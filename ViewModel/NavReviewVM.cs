﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using StdEqpTesting.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StdEqpTesting.ViewModel
{
	public partial class NavReviewVM : ObservableObject
	{
		[ObservableProperty]
		object _DataGridSource;

		int _DataTypeIndex = 0;
		public int DataTypeIndex
		{
			get => _DataTypeIndex;
			set
			{
				DataGridSource = (_DataTypeIndex = value) switch
				{
					0 => COMDataGridSource,
					1 => DISPDataGridSource,
					_ => throw new NotImplementedException(),
				};
			}
		}
		public string SearchString { get; set; }

		ObservableCollection<COMDataGridModel> COMDataGridSource = new ObservableCollection<COMDataGridModel>();
		ObservableCollection<DISPDataGridModel> DISPDataGridSource = new ObservableCollection<DISPDataGridModel>();

		private bool Reading;
		[RelayCommand]
		public void ReadDB(int typeIndex)
		{
			Reading = true;
			switch (typeIndex)
			{
				case 0: //COM Data
					COMDataGridSource.Clear();
					using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
					{
						connection.Open();
						using SqliteCommand command = connection.CreateCommand();
						command.CommandText = $"SELECT * FROM ComTestData WHERE TestName LIKE '%{SearchString}%'";
						using SqliteDataReader reader = command.ExecuteReader();
						while (reader.Read())
							COMDataGridSource.Add(new COMDataGridModel(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetInt64(8)));
					}
					break;
				case 1: //Displacement Data
					DISPDataGridSource.Clear();
					using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
					{
						connection.Open();
						using SqliteCommand command = connection.CreateCommand();
						command.CommandText = $"SELECT * FROM DispTestData WHERE TestName LIKE '%{SearchString}%'";
						using SqliteDataReader reader = command.ExecuteReader();
						while (reader.Read())
							DISPDataGridSource.Add(new DISPDataGridModel(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetInt64(6)));
					}
					break;
				default:
					throw new NotImplementedException();
			}
			Reading = false;
		}

		#region DataGrid row add/remove
		private void COMDataGridSource_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{   //Row added or removed.
			if (Reading) return;    //Don't do this if DB Reading is in progress.
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				using SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString());
				connection.Open();
				using SqliteCommand command = connection.CreateCommand();
				command.CommandText = "DELETE FROM ComTestData WHERE ID = $ID";
				command.Parameters.AddWithValue("$ID", ((COMDataGridModel)e.OldItems[0]).ID);
				command.ExecuteNonQuery();
			}

		}
		private void DISPDataGridSource_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (Reading) return;
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				using SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString());
				connection.Open();
				using SqliteCommand command = connection.CreateCommand();
				command.CommandText = "DELETE FROM DispTestData WHERE ID = $ID";
				command.Parameters.AddWithValue("$ID", ((DISPDataGridModel)e.OldItems[0]).ID);
				command.ExecuteNonQuery();
			}

		}
		#endregion

		public NavReviewVM()
		{
			Task.Run(() =>
			{
				ReadDB(0);
				ReadDB(1);
			});
			COMDataGridSource.CollectionChanged += COMDataGridSource_CollectionChanged;
			DISPDataGridSource.CollectionChanged += DISPDataGridSource_CollectionChanged;
			DataGridSource = COMDataGridSource; //Index 0
		}
	}
}
