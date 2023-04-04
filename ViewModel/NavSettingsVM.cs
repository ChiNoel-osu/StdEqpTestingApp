using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace StdEqpTesting.ViewModel
{
	public partial class NavSettingsVM : ObservableObject
	{
		bool init = true;

		int _LangIndex;
		public int LangIndex
		{
			get => _LangIndex;
			set
			{
				Properties.Settings.Default.Locale = (_LangIndex = value) switch
				{
					0 => "zh-CN",
					1 => "en-US",
					_ => throw new NotImplementedException(),
				};
				Properties.Settings.Default.Save();
				CultureInfo.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Locale);
				if (!init && MessageBox.Show(Localization.Loc.SettingLangSaved, Localization.Loc.SettingLang, MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No) == MessageBoxResult.Yes)
				{   //Restart to take effect.
					Process.Start(Process.GetCurrentProcess().MainModule.FileName);
					Application.Current.Shutdown();
				}
			}
		}

		public string ImageSaveDir
		{
			get
			{
				return Properties.Settings.Default.ImageSaveDir;
			}
			set
			{
				Properties.Settings.Default.ImageSaveDir = value;
				Properties.Settings.Default.Save();
			}
		}

		public int JPEGQuality
		{
			get
			{
				return Properties.Settings.Default.ImageSaveQuality;
			}
			set
			{
				Properties.Settings.Default.ImageSaveQuality = value;
				Properties.Settings.Default.Save();
			}
		}

		#region COM Setting Prop
		string _SelectedPort;
		public string SelectedPort
		{
			get => _SelectedPort;
			set
			{
				GetCOMSetting(_SelectedPort = value);
			}
		}
		public int SelectedCOMIndex { get; set; }   //To make the view remember selection.
		[ObservableProperty]
		int _BaudRate = -1;
		[ObservableProperty]
		int _ParityOrdinal = -1;
		[ObservableProperty]
		int _DataBits = -1;
		[ObservableProperty]
		int _StopBitsOrdinal = -1;
		[ObservableProperty]
		int _HandshakeOrdinal = -1;
		[ObservableProperty]
		string _EncodingStr;
		[ObservableProperty]
		int _ReadTimeout = -1;
		[ObservableProperty]
		int _WriteTimeout = -1;
		[ObservableProperty]
		ObservableCollection<string> _COMSettingPorts = new ObservableCollection<string>();
		#endregion

		public bool saved;
		public void SaveCOMSettings(string name)
		{
			saved = false;
			if (name is null) return;
			if (name == Localization.Loc.SettingDefaultCOM) name = "NewCOMDefault";
			File.WriteAllText(Path.Combine(Properties.Settings.Default.ConfigFolderDir, "COM", name + "SP.json"), JsonSerializer.Serialize(new Model.COMPortPropModel()
			{
				BaudRate = BaudRate,
				Parity = (Parity)ParityOrdinal,
				DataBits = DataBits,
				StopBits = (StopBits)StopBitsOrdinal,
				Handshake = (Handshake)HandshakeOrdinal,
				EncodingString = EncodingStr,
				ReadTimeout = ReadTimeout,
				WriteTimeout = WriteTimeout
			}));
			MainViewModel.MainVM.UpdateMainStatus(Localization.Loc.COMSettingSaved.Replace("%PortName", name), true);
			saved = true;
		}

		public void GetCOMSetting(string name)
		{
			string configDir = Properties.Settings.Default.ConfigFolderDir;
			string filePath;
			//Generate COM Default json if not exist.
			if (!File.Exists(filePath = Path.Combine(configDir, "COM", "NewCOMDefaultSP.json")))
			{
				App.Logger.Warn("New COM Port Default setting file not found, creating one.");
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
				File.WriteAllText(filePath, JsonSerializer.Serialize(new Model.COMPortPropModel()
				{
					BaudRate = 9600,
					Parity = Parity.None,
					DataBits = 8,
					StopBits = StopBits.One,
					Handshake = Handshake.None,
					EncodingString = Encoding.ASCII.WebName,
					ReadTimeout = 1000,
					WriteTimeout = 1000
				}));
			}
			//Generate individual COM prop json if not exist.
			foreach (string portName in SerialPort.GetPortNames())
				//COM1~COM9 cannot be used as file names as they're reserved by system.
				//Adding "SP" as a suffix here.
				if (!File.Exists(filePath = Path.Combine(configDir, "COM", portName + "SP.json")))
				{
					App.Logger.Warn(portName + "setting file not found, copying from NewCOMDefaultSP.json.");
					Directory.CreateDirectory(Path.GetDirectoryName(filePath));
					File.Copy(Path.Combine(configDir, "COM", "NewCOMDefaultSP.json"), filePath);
				}
			//Read config
			if (init || name is null) return;
			if (name == Localization.Loc.SettingDefaultCOM) name = "NewCOMDefault";
			Model.COMPortPropModel setting = JsonSerializer.Deserialize<Model.COMPortPropModel>(File.ReadAllText(Path.Combine(configDir, "COM", name + "SP.json")));
			BaudRate = setting.BaudRate;
			ParityOrdinal = (int)setting.Parity;
			DataBits = setting.DataBits;
			StopBitsOrdinal = (int)setting.StopBits;
			HandshakeOrdinal = (int)setting.Handshake;
			EncodingStr = setting.EncodingString;
			ReadTimeout = setting.ReadTimeout;
			WriteTimeout = setting.WriteTimeout;
		}

		public void GetImgSetting()
		{

		}

		public NavSettingsVM()
		{
			GetCOMSetting("NewCOMDefault");

			LangIndex = Properties.Settings.Default.Locale switch
			{
				"zh-CN" => 0,
				"en-US" => 1,
				_ => 0  //This is handled in App.xaml.cs (Reverting to zh-CN)
			};

			COMSettingPorts.Add(Localization.Loc.SettingDefaultCOM);
			foreach (string portName in SerialPort.GetPortNames())
				COMSettingPorts.Add(portName);

			init = false;
		}
	}
}
