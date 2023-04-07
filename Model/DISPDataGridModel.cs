using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.Sqlite;
using StdEqpTesting.ViewModel;
using System;
using System.Windows;

namespace StdEqpTesting.Model
{
	/// <summary>
	/// Represents the displacement data in the Review section.
	/// </summary>
	public partial class DISPDataGridModel : ObservableObject
	{
		public int ID { get; private set; }
		[ObservableProperty]
		string _User = string.Empty;
		[ObservableProperty]
		string _TestName = string.Empty;
		[ObservableProperty]
		string _TestValue = string.Empty;
		[ObservableProperty]
		string _TestUnit = string.Empty;
		[ObservableProperty]
		string? _Tag;
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
		private void DISPDataGridModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{   //Data Grid updated, update DB as well.
			App.Logger.Info($"Updating DB DispTestData {e.PropertyName}={this.GetType().GetProperty(e.PropertyName).GetValue(this)}, ID:{ID}");
			using SqliteConnection connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Properties.Settings.Default.DBConnString, Mode = SqliteOpenMode.ReadWrite }.ToString());
			connection.Open();
			using SqliteCommand command = connection.CreateCommand();
			command.CommandText = $"UPDATE DispTestData SET {e.PropertyName}=$STH WHERE ID = $ID";
			command.Parameters.AddWithValue("$ID", this.ID);
			command.Parameters.AddWithValue("$STH", this.GetType().GetProperty(e.PropertyName).GetValue(this).ToString());
			try
			{
				command.ExecuteNonQuery();
				MainViewModel.MainVM.UpdateMainStatus(Localization.Loc.Saved2DB, true);
			}
			catch (SqliteException ex)
			{
				MainViewModel.MainVM.UpdateMainStatus(Localization.Loc.NotSaved2DB, true, 3);
				MessageBox.Show(Localization.Loc.SQLReviewEx.Replace("%Exception", ex.Message), "SQL command failed.", MessageBoxButton.OK, MessageBoxImage.Warning); ;
			}
		}
		public string[] ToStringArray() => new string[] { ID.ToString(), (string)Time, this.User, this.TestName, this.TestValue, this.TestUnit, this.Tag };
		public DISPDataGridModel(int ID, string User, string TestName, string TestValue, string TestUnit, string? Tag, long UnixTimeStamp)
		{
			this.ID = ID;
			this.User = User;
			this.TestName = TestName;
			this.TestValue = TestValue;
			this.TestUnit = TestUnit;
			this.Tag = Tag;
			this.Time = UnixTimeStamp;

			this.PropertyChanged += DISPDataGridModel_PropertyChanged;
		}
	}
}
