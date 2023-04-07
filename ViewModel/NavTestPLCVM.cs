using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StdEqpTesting.ViewModel
{
	public partial class NavTestPLCVM : ObservableObject
	{
		#region Visualization
		bool _Q00;
		public object Q00
		{
			get => _Q00.ToString();
			set { _Q00 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q00)); }
		}
		bool _Q01;
		public object Q01
		{
			get => _Q01.ToString();
			set { _Q01 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q01)); }
		}
		bool _Q02;
		public object Q02
		{
			get => _Q02.ToString();
			set { _Q02 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q02)); }
		}
		bool _Q03;
		public object Q03
		{
			get => _Q03.ToString();
			set { _Q03 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q03)); }
		}
		bool _Q04;
		public object Q04
		{
			get => _Q04.ToString();
			set { _Q04 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q04)); }
		}
		bool _Q05;
		public object Q05
		{
			get => _Q05.ToString();
			set { _Q05 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q05)); }
		}
		bool _Q06;
		public object Q06
		{
			get => _Q06.ToString();
			set { _Q06 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q06)); }
		}
		bool _Q07;
		public object Q07
		{
			get => _Q07.ToString();
			set { _Q07 = bool.Parse(value.ToString()); OnPropertyChanged(nameof(Q07)); }
		}
		#endregion
		bool _SimStatus = false;
		public object SimStatus
		{
			get => _SimStatus.ToString();
			set { _SimStatus = bool.Parse(value.ToString()); OnPropertyChanged(nameof(SimStatus)); }
		}

		[RelayCommand]
		public void LoadILS()
		{
			App.Logger.Info("Selecting ILS File to load.");
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
			openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.Title = Localization.Loc.PLCOpenILS;
			if ((bool)openFileDialog.ShowDialog())
			{
				try
				{
					ILSText = File.ReadAllText(ILSPath = openFileDialog.FileName);
					MainViewModel.MainVM.UpdateSecStatus(Localization.Loc.PLCLoadedILS.Replace("%Path", openFileDialog.FileName), true);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
					App.Logger.Error(ex.Message);
					MainViewModel.MainVM.UpdateSecStatus(Localization.Loc.PLCNotLoadILS, true, 4);
				}
			}
			else
			{
				MainViewModel.MainVM.UpdateSecStatus(Localization.Loc.PLCNotLoadILS, true);
			}
		}
		[ObservableProperty]
		string _ILSText = string.Empty;
		[ObservableProperty]
		string _Updating = "Nope";

		string ILSPath = string.Empty;

		CancellationTokenSource cts;
		CancellationToken ct;
		[RelayCommand]
		public void SimToggle()
		{
			if ((bool)(SimStatus = !_SimStatus))
			{
				Task.Run(() =>
				{
					cts = new CancellationTokenSource();
					ct = cts.Token;
					StuffHappens(ct);   //No need for ref keyword
				});
				App.Logger.Info("PLC Simulation has started.");
			}
			else
			{
				cts.Cancel();
				cts.Dispose();
				App.Logger.Info("PLC Simulation has stopped.");
			}
		}
		void StuffHappens(CancellationToken ct)
		{
			while (true)
			{
				ct.ThrowIfCancellationRequested();
				ILSText = File.ReadAllText(ILSPath);
				string[] ILSbyLine = ILSText.Split('\n');
				string[,] ILS2D = new string[ILSbyLine.Length, 2];
				for (ushort i = 0; i < ILSbyLine.Length; i++)
				{   //Put everything in a 2D string array.
					string[] op = ILSbyLine[i].Split(' ');
					ILS2D[i, 0] = op[0].Trim();
					if (op.Length > 1)
						ILS2D[i, 1] = op[1].Trim();
				}
				Application.Current.Dispatcher.Invoke(() => { Updating = "Update"; });
				for (ushort i = 0; i < ILS2D.GetLength(0); i++)
					this.GetType().GetProperty(ILS2D[i, 0]).SetValue(this, ILS2D[i, 1]);
				Application.Current.Dispatcher.Invoke(() => { Updating = "Nope"; });
				Thread.Sleep(Properties.Settings.Default.PLCUpdateInterval);
			}
		}
	}
}
