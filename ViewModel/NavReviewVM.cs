using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using StdEqpTesting.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
					1 => COMDataGridSource2,
					_ => throw new NotImplementedException(),
				};
			}
		}

		ObservableCollection<COMDataGridModel> COMDataGridSource = new ObservableCollection<COMDataGridModel>();
		//TODO: Prox and Img
		ObservableCollection<COMDataGridModel> COMDataGridSource2 = new ObservableCollection<COMDataGridModel>();

		[RelayCommand]
		public void ReadDB(int typeIndex)
		{
			switch (typeIndex)
			{
				case 0: //COM Data
					COMDataGridSource.Clear();
					using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
					{
						connection.Open();
						using (SqliteCommand command = connection.CreateCommand())
						{
							command.CommandText = "SELECT * FROM ComTestData";
							using (SqliteDataReader reader = command.ExecuteReader())
							{
								while (reader.Read())
									COMDataGridSource.Add(new COMDataGridModel(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetInt64(8)));
							}
						}
					}
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void COMDataGridSource_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{	//Row added or removed.

		}

		public NavReviewVM()
		{
			Task.Run(() =>
			{
				ReadDB(0);
			});
			COMDataGridSource.CollectionChanged += COMDataGridSource_CollectionChanged;
			COMDataGridSource2.Add(new COMDataGridModel(1, "bruh", "Name3", null, "66", "V", null, null, 1679672308));
			DataGridSource = COMDataGridSource; //Index 0
		}
	}
}
