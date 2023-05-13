using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StdEqpTesting.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StdEqpTesting.ViewModel
{
	public partial class NavTestVM : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<TestTabItemModel> _TabItemSource = new ObservableCollection<TestTabItemModel>();
		[ObservableProperty]
		int _TabIndex = 0;

		public List<string> COMList = new List<string>();

		readonly TestTabItemModel noCOM = new TestTabItemModel(false) { PortName = Localization.Loc.NoCOMTabHeader };
		[RelayCommand]
		public void ReGetCOMList()
		{
			App.Logger.Info("COM list refresh requested.");
			int tempTabIndex = TabIndex;    //Can have minor visual bugs.
			string[] portNames = SerialPort.GetPortNames();
			if (portNames.Length >= COMList.Count)  //The number of ports has increased or not changed.
				foreach (string COM in portNames)
				{
					if (COMList.Contains(COM))
						continue;   //Skip existing ports.
					COMList.Add(COM);   //Add missing ports.
					TabItemSource.Add(new TestTabItemModel() { PortName = COM });
				}
			else    //The number of ports has decreased.
				foreach (string COM in COMList.ToArray())
				{   //ToArray() makes a copy of COMList so it won't change during the enumeration.
					if (portNames.Contains(COM))
						continue;   //The port still exists, don't delete.
					COMList.Remove(COM);    //Remove missing ports.
					TabItemSource.Remove(TabItemSource.First(p => p.PortName == COM));
				}
			if (COMList.Count == 0 && !TabItemSource.Contains(noCOM))
			{   //No COM device.
				TabItemSource.Add(noCOM);
				tempTabIndex = 0;
			}
			else if (COMList.Count != 0)
				TabItemSource.Remove(noCOM);
			TabIndex = tempTabIndex;
			MainViewModel.MainVM?.UpdateMainStatus(Localization.Loc.FoundCOMDevices.Replace("%Num", COMList.Count.ToString()), true);
			MainViewModel.MainVM?.NavSettingsVM.InitCOMSettingsBox();
		}

		public NavTestVM()
		{
			ReGetCOMList();
			//HACK: Update status. Delay 1 sec otherwise MainVM would be null.
			Task.Run(() =>
			{
				Thread.Sleep(1000);
				MainViewModel.MainVM.UpdateMainStatus(Localization.Loc.FoundCOMDevices.Replace("%Num", COMList.Count.ToString()), true);
			});
		}
	}
}
