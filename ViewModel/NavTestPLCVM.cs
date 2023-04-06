using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StdEqpTesting.ViewModel
{
	public partial class NavTestPLCVM : ObservableObject
	{
		#region SB & Q
		bool _SB1EN;
		public object SB1EN
		{
			get => _SB1EN.ToString();
			set { _SB1EN = bool.Parse(value.ToString()); OnPropertyChanged(nameof(SB1EN)); }
		}
		bool _SB2EN;
		public object SB2EN
		{
			get => _SB2EN.ToString();
			set { _SB2EN = bool.Parse(value.ToString()); OnPropertyChanged(nameof(SB2EN)); }
		}
		bool _SB3EN;
		public object SB3EN
		{
			get => _SB3EN.ToString();
			set { _SB3EN = bool.Parse(value.ToString()); OnPropertyChanged(nameof(SB3EN)); }
		}
		bool _SB4EN;
		public object SB4EN
		{
			get => _SB4EN.ToString();
			set { _SB4EN = bool.Parse(value.ToString()); OnPropertyChanged(nameof(SB4EN)); }
		}
		bool _SB5EN;
		public object SB5EN
		{
			get => _SB5EN.ToString();
			set { _SB5EN = bool.Parse(value.ToString()); OnPropertyChanged(nameof(SB5EN)); }
		}
		bool _SB6EN;
		public object SB6EN
		{
			get => _SB6EN.ToString();
			set { _SB6EN = bool.Parse(value.ToString()); OnPropertyChanged(nameof(SB6EN)); }
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
					ILSText = File.ReadAllText(openFileDialog.FileName);
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
			string[] ILSbyLine = ILSText.Split('\n');
			string[,] ILS2D = new string[ILSbyLine.Length, 2];
			for (ushort i = 0; i < ILSbyLine.Length; i++)
			{   //Put everything in a 2D string array.
				string[] op = ILSbyLine[i].Split(' ');
				ILS2D[i, 0] = op[0].Trim();
				if (op.Length > 1)
					ILS2D[i, 1] = op[1].Trim();
			}
			Stack<string> oprndStack = new Stack<string>();
			for (ushort i = 0; i < ILS2D.GetLength(0); i++)
			{   //Start processing stuff.
				string oprtr = ILS2D[i, 0];
				string? oprnd = ILS2D[i, 1];
				switch (oprtr)
				{
					case "LD":  //Load
						oprndStack.Push(oprnd);
						break;
					case "LDI": //Load inverted
						break;
					case "AND": //AND
						break;
					case "ANI": //AND inverted
						break;
					case "OR":  //OR
						break;
					case "ORI": //OR inverted
						break;
					case "INV": //Invert result (no operand)
						break;
					case "OUT": //Result
						break;
					case "END": //End of ILS (no operand)
						break;
					default:
						throw new NotImplementedException();
				}
			}
			while (true)
			{
				ct.ThrowIfCancellationRequested();
			}
		}

		[RelayCommand]
		public void SBToggle(string num)
		{
			switch (byte.Parse(num))
			{
				case 1:
					SB1EN = !_SB1EN;
					break;
				case 2:
					SB2EN = !_SB2EN;
					break;
				case 3:
					SB3EN = !_SB3EN;
					break;
				case 4:
					SB4EN = !_SB4EN;
					break;
				case 5:
					SB5EN = !_SB5EN;
					break;
				case 6:
					SB6EN = !_SB6EN;
					break;
			}
		}
	}
}
