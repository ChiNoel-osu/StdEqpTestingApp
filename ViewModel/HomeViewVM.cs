using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StdEqpTesting.Model;
using StdEqpTesting.View;
using System;

namespace StdEqpTesting.ViewModel
{
	public partial class HomeViewVM : ObservableObject
	{
		public string UserName { get; set; }
		public UserTypeEnum UserType { get; set; }

		[ObservableProperty]
		System.Windows.Controls.UserControl _Content = navTest;

		#region Navigation Stuff
		static readonly NavTest navTest = new NavTest();
		static readonly NavTestDISP navTestDISP = new NavTestDISP();
		static readonly NavTestImg navTestImg = new NavTestImg();
		static readonly NavTestPLC navTestPLC = new NavTestPLC();
		static readonly NavReview navReview = new NavReview();
		static readonly NavExport navExport = new NavExport();
		static readonly NavSettings navSettings = new NavSettings();

		[ObservableProperty]
		bool _NavTestChecked = true;
		partial void OnNavTestCheckedChanged(bool value) { if (NavTestChecked = value) Content = navTest; }
		[ObservableProperty]
		bool _NavTestDispChecked = false;
		partial void OnNavTestDispCheckedChanged(bool value) { if (NavTestDispChecked = value) Content = navTestDISP; }
		[ObservableProperty]
		bool _NavTestImgChecked = false;
		partial void OnNavTestImgCheckedChanged(bool value) { if (NavTestImgChecked = value) Content = navTestImg; }
		[ObservableProperty]
		bool _NavTestPLCChecked = false;
		partial void OnNavTestPLCCheckedChanged(bool value) { if (NavTestPLCChecked = value) Content = navTestPLC; }
		[ObservableProperty]
		bool _NavReviewChecked = false;
		partial void OnNavReviewCheckedChanged(bool value) { if (NavReviewChecked = value) Content = navReview; }
		[ObservableProperty]
		bool _NavExportChecked = false;
		partial void OnNavExportCheckedChanged(bool value) { if (NavExportChecked = value) Content = navExport; }
		[ObservableProperty]
		bool _NavSettingsChecked = false;
		partial void OnNavSettingsCheckedChanged(bool value) { if (NavSettingsChecked = value) Content = navSettings; }

		[RelayCommand]
		public void TabNavigate(string TabNumber)
		{
			switch (TabNumber)
			{
				case "1": Content = navTest; NavTestChecked = true; break;
				case "2": Content = navTestDISP; NavTestDispChecked = true; break;
				case "3": Content = navTestImg; NavTestImgChecked = true; break;
				case "4": Content = navTestPLC; NavTestPLCChecked = true; break;
				case "5": Content = navReview; NavReviewChecked = true; break;
				case "6": Content = navExport; NavExportChecked = true; break;
				case "O": Content = navSettings; NavSettingsChecked = true; break;
				default: throw new NotImplementedException();
			}
		}
		#endregion
	}
}
