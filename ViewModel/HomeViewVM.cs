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
		static readonly NavReview navReview = new NavReview();
		static readonly NavExport navExport = new NavExport();
		static readonly NavSettings navSettings = new NavSettings();
		bool _NavTestChecked = true;
		public bool NavTestChecked
		{
			get => _NavTestChecked;
			set
			{
				if (_NavTestChecked = value) Content = navTest;
				OnPropertyChanged(nameof(NavTestChecked));
			}
		}
		bool _NavTestDispChecked = false;
		public bool NavTestDispChecked
		{
			get => _NavTestDispChecked;
			set
			{
				if (_NavTestDispChecked = value) Content = navTestDISP;
				OnPropertyChanged(nameof(NavTestDispChecked));
			}
		}
		bool _NavTestImgChecked = false;
		public bool NavTestImgChecked
		{
			get => _NavTestImgChecked;
			set
			{
				if (_NavTestImgChecked = value) Content = navTestImg;
				OnPropertyChanged(nameof(NavTestImgChecked));
			}
		}
		bool _NavReviewChecked = false;
		public bool NavReviewChecked
		{
			get => _NavReviewChecked;
			set
			{
				if (_NavReviewChecked = value) Content = navReview;
				OnPropertyChanged(nameof(NavReviewChecked));
			}
		}
		bool _NavExportChecked = false;
		public bool NavExportChecked
		{
			get => _NavExportChecked;
			set
			{
				if (_NavExportChecked = value) Content = navExport;
				OnPropertyChanged(nameof(NavExportChecked));
			}
		}
		bool _NavSettingsChecked = false;
		public bool NavSettingsChecked
		{
			get => _NavSettingsChecked;
			set
			{
				if (_NavSettingsChecked = value) Content = navSettings;
				OnPropertyChanged(nameof(NavSettingsChecked));
			}
		}
		[RelayCommand]
		public void TabNavigate(string TabNumber)
		{
			switch (TabNumber)
			{
				case "1": Content = navTest; NavTestChecked = true; break;
				case "2": Content = navTestDISP; NavTestDispChecked = true; break;
				case "3": Content = navTestImg; NavTestImgChecked = true; break;
				case "4": Content = navReview; NavReviewChecked = true; break;
				case "5": Content = navExport; NavExportChecked = true; break;
				case "O": Content = navSettings; NavSettingsChecked = true; break;
				default: throw new NotImplementedException();
			}
		}
		#endregion

		public HomeViewVM()
		{

		}
	}
}
