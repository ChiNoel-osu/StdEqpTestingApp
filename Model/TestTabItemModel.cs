using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StdEqpTesting.Model
{
	public partial class TestTabItemModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<TestDataModel> _DataListBox = new ObservableCollection<TestDataModel>();
		[ObservableProperty]
		bool _PortOpened = false;
		[ObservableProperty]
		bool _PropExpanded = true;
		[ObservableProperty]
		string _Message;

		[RelayCommand]
		public void Connect()
		{
			if (PortOpened)
			{
				cts.Cancel();
				serialPort.Close();
				cts.Dispose();
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
				}
				catch (Exception e)
				{   //TODO: Make it pretty.
					MessageBox.Show(e.Message);
				}
			}
			PortOpened = serialPort.IsOpen;
		}

		CancellationTokenSource cts;
		CancellationToken ct;

		SerialPort serialPort = new SerialPort();

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
		}
	}
}
