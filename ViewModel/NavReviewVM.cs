using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using StdEqpTesting.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
						try
						{
							using SqliteDataReader reader = command.ExecuteReader();
							while (reader.Read())
								COMDataGridSource.Add(new COMDataGridModel(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetInt64(8)));
						}
						catch (SqliteException ex)
						{
							MessageBox.Show(ex.Message);
							MainViewModel.MainVM.UpdateMainStatus(Localization.Loc.SearchFail, true, 4);
						}
					}
					break;
				case 1: //Displacement Data
					DISPDataGridSource.Clear();
					using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
					{
						connection.Open();
						using SqliteCommand command = connection.CreateCommand();
						command.CommandText = $"SELECT * FROM DispTestData WHERE TestName LIKE '%{SearchString}%'";
						try
						{
							using SqliteDataReader reader = command.ExecuteReader();
							while (reader.Read())
								DISPDataGridSource.Add(new DISPDataGridModel(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetInt64(6)));
						}
						catch (SqliteException ex)
						{
							MessageBox.Show(ex.Message);
							MainViewModel.MainVM.UpdateMainStatus(Localization.Loc.SearchFail, true, 4);
						}
					}
					break;
				default:
					throw new NotImplementedException();
			}
			Reading = false;
		}

		[RelayCommand]
		public void ExportCSV(DataGrid dataGrid)
		{
			StringBuilder csvString = new StringBuilder();
			//Header
			csvString.AppendJoin(',', from col in dataGrid.Columns select col.Header);
			csvString.Append('\n');
			//Data
			foreach (DataGridRow dataGridRow in GetDataGridRows(dataGrid))
			{
				switch (dataGridRow.Item)
				{
					case COMDataGridModel _:
						csvString.AppendJoin(',', ((COMDataGridModel)dataGridRow.Item).ToStringArray());
						break;
					case DISPDataGridModel _:
						csvString.AppendJoin(',', ((DISPDataGridModel)dataGridRow.Item).ToStringArray());
						break;
					default:
						throw new NotImplementedException();
				}
				//if (dataGridRow.Item.GetType() == typeof(COMDataGridModel))
				//	csvString.AppendJoin(',', ((COMDataGridModel)dataGridRow.Item).ToStringArray());
				csvString.Append('\n');
			}
			//Prompt for path.
			SaveFileDialog saveCSVDialog = new SaveFileDialog()
			{
				InitialDirectory = Directory.GetCurrentDirectory(),
				ValidateNames = true,
				FilterIndex = 0,
				Filter = "CSV Files (*.csv)|*.csv",
				Title = Localization.Loc.ExportCSV
			};
			if ((bool)saveCSVDialog.ShowDialog())
			{
				File.WriteAllTextAsync(saveCSVDialog.FileName, csvString.ToString(), Encoding.UTF8);
				MainViewModel.MainVM.UpdateSecStatus(Localization.Loc.ExportedCSV.Replace("%Path", saveCSVDialog.FileName), true);
			}
			else
			{
				MainViewModel.MainVM.UpdateSecStatus(Localization.Loc.NotExported, true);
			}
		}
		IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
		{
			IEnumerable? itemsSource = grid.ItemsSource;
			if (null == itemsSource) yield return null;
			foreach (object? item in itemsSource)
			{
				DataGridRow? row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
				if (null != row) yield return row;
			}
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
			//Task.Run(() =>
			//{
			//	ReadDB(0);
			//	ReadDB(1);
			//});
			COMDataGridSource.CollectionChanged += COMDataGridSource_CollectionChanged;
			DISPDataGridSource.CollectionChanged += DISPDataGridSource_CollectionChanged;
			DataGridSource = COMDataGridSource; //Index 0
		}
	}
}
