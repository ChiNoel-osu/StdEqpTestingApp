using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StdEqpTesting.Localization;
using StdEqpTesting.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StdEqpTesting.Model
{
	//It's more like a ViewModel now but whatever.
	public partial class TestTabItemModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<TestDataModel> _DataListBox = new ObservableCollection<TestDataModel>();
		[ObservableProperty]
		string _Message;
		[ObservableProperty]
		ObservableCollection<string> _UnitList = new ObservableCollection<string>();
		[ObservableProperty]
		bool _PortOpened = false;

		//Props that doesn't need to be observed (not updating constantly on UI).
		public bool PropExpanded { get; set; } = false;
		public string AdditionalInfo { get; set; } = string.Empty;
		public bool AutoAdd { get; set; } = false;
		public bool AutoClear { get; set; } = true;
		public string TestName { get; set; } = string.Empty;
		public string MeaUnit { get; set; } = string.Empty;

		public bool IsUnitAdded = false;    //Animation in code-behind.

		CancellationTokenSource cts;
		CancellationToken ct;
		SerialPort serialPort = new SerialPort();
		[RelayCommand]
		public void Connect()
		{
			if (PortOpened)
			{
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
					{
						while (true)
						{
							ct.ThrowIfCancellationRequested();  //This will throw an exception, which is not handled.
							Message += serialPort.ReadExisting();
							Thread.Sleep(100);
						}
					}, ct);
					MainViewModel.MainVM.LogSth($"Serial port {PortName} opened.", 2);
				}
				catch (Exception e)
				{   //TODO: Make it pretty.
					MainViewModel.MainVM.LogSth(e.ToString(), 4);
					MessageBox.Show(e.Message);
				}
			}
			PortOpened = serialPort.IsOpen;
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

		public TestTabItemModel(bool hasCOM = true)
		{
			NoCOM = !hasCOM;
			//Read units from file.
			if (File.Exists($"Database{Path.DirectorySeparatorChar}Units.txt"))
			{
				string[] unitsInFile = File.ReadAllLines($"Database{Path.DirectorySeparatorChar}Units.txt");
				_UnitList.Clear();
				foreach (string unit in unitsInFile)
					UnitList.Add(unit);
			}
		}
	}
}
