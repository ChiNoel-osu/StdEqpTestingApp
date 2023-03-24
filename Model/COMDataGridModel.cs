using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.Sqlite;
using System;
//TODO: Logging
namespace StdEqpTesting.Model
{
	public partial class COMDataGridModel : ObservableObject
	{
		public int ID { get; private set; }
		[ObservableProperty]
		string _User = string.Empty;
		[ObservableProperty]
		string _TestName = string.Empty;
		[ObservableProperty]
		string? _ValueType;
		[ObservableProperty]
		string _TestValue = string.Empty;
		[ObservableProperty]
		string _TestUnit = string.Empty;
		[ObservableProperty]
		string? _Tag;
		[ObservableProperty]
		string? _COMPort;
		long _Time;
		public object Time
		{
			get
			{
				return DateTimeOffset.FromUnixTimeSeconds(_Time).LocalDateTime.ToString("yyyy/MM/dd HH:mm:ss");
			}
			private set
			{
				_Time = (long)value;
			}
		}

		public COMDataGridModel(int ID, string User, string TestName, string? ValueType, string TestValue, string TestUnit, string? Tag, string? COMPort, long UnixTimeStamp)
		{
			this.ID = ID;
			this.User = User;
			this.TestName = TestName;
			this.ValueType = ValueType;
			this.TestValue = TestValue;
			this.TestUnit = TestUnit;
			this.Tag = Tag;
			this.COMPort = COMPort;
			this.Time = UnixTimeStamp;

			this.PropertyChanged += COMDataGridModel_PropertyChanged;
		}

		private void COMDataGridModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			using (SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString()))
			{
				connection.Open();
				using (SqliteCommand command = connection.CreateCommand())
				{
					command.CommandText = $"UPDATE ComTestData SET {e.PropertyName}=${e.PropertyName} WHERE ID = $ID";
					command.Parameters.AddWithValue($"${e.PropertyName}", this.GetType().GetProperty(e.PropertyName).GetValue(this).ToString());
					command.Parameters.AddWithValue("$ID", this.ID);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}
