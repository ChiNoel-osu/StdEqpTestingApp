using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace StdEqpTesting.ViewModel
{
	public partial class MainViewModel : ObservableObject
	{
		public static MainViewModel MainVM { get; private set; }

		public HomeViewVM HomeViewVM { get; } = new HomeViewVM();
		public NavTestVM NavTestVM { get; } = new NavTestVM();
		public NavTestDispVM NavTestDispVM { get; } = new NavTestDispVM();
		public NavTestImgVM NavTestImgVM { get; } = new NavTestImgVM();
		public NavTestPLCVM NavTestPLCVM { get; } = new NavTestPLCVM();
		public NavReviewVM NavReviewVM { get; } = new NavReviewVM();
		public NavSettingsVM NavSettingsVM { get; } //Late load.

		//Accessing any ObservableProperty of this class will trigger CS0229 (in VS).
		//But the code still work and no exceptions are thorn.
		[ObservableProperty]
		string _MainStatus = Localization.Loc.StatusReady;
		[ObservableProperty]
		string _SecondaryStatus = Localization.Loc.MainWndTitle;

		/// <summary>
		/// Updates the main status on the HomeViewWindow.
		/// </summary>
		/// <param name="status">The status string.</param>
		/// <param name="logStatus">Also log the status to file.</param>
		/// <param name="logLevel">Level of log. Default: Info.</param>
		public void UpdateMainStatus(string status, bool logStatus = false, byte logLevel = 2)
		{
			MainStatus = status;
			if (logStatus)
				LogSth(status, logLevel);
		}
		/// <summary>
		/// Updates the secondary status on the HomeViewWindow.
		/// </summary>
		/// <param name="status">The status string.</param>
		/// <param name="logStatus">Also log the status to file.</param>
		/// <param name="logLevel">Level of log. Default: Info.</param>
		public void UpdateSecStatus(string status, bool logStatus = false, byte logLevel = 2)
		{
			SecondaryStatus = status;
			if (logStatus)
				LogSth(status, logLevel);
		}

		public static void LogSth(string logString, byte logLevel)
		{
			switch (logLevel)
			{
				case 0:
					App.Logger.Trace(logString);
					break;
				case 1:
					App.Logger.Debug(logString);
					break;
				case 2:
					App.Logger.Info(logString);
					break;
				case 3:
					App.Logger.Warn(logString);
					break;
				case 4:
					App.Logger.Error(logString);
					break;
				case 5:
					App.Logger.Fatal(logString);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public MainViewModel(Model.UserInfo userInfo)
		{
			MainVM = this;
			HomeViewVM.UserName = userInfo.username;
			HomeViewVM.UserType = userInfo.type;
			SecondaryStatus = Localization.Loc.StatusLoggedIn.Replace("%User", userInfo.username).Replace("%Type", userInfo.type.ToString());
			NavSettingsVM = new NavSettingsVM(userInfo);
		}
	}
}
