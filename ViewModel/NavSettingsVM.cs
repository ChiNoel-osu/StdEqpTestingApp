using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace StdEqpTesting.ViewModel
{
	public class NavSettingsVM
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
				{	//Restart to take effect.
					Process.Start(Process.GetCurrentProcess().MainModule.FileName);
					Application.Current.Shutdown();
				}
			}
		}

		public NavSettingsVM()
		{
			LangIndex = Properties.Settings.Default.Locale switch
			{
				"zh-CN" => 0,
				"en-US" => 1,
				_ => 0  //This is handled in App.xaml.cs (Reverting to zh-CN)
			};
			init = false;
		}
	}
}
