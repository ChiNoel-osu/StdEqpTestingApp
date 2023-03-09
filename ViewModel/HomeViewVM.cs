using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StdEqpTesting.Model;
using StdEqpTesting.View;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StdEqpTesting.ViewModel
{
	public partial class HomeViewVM : ObservableObject
	{
		public string UserName { get; set; }
		public UserTypeEnum UserType { get; set; }

		[ObservableProperty]
		System.Windows.Controls.UserControl _Content = navTest;
		[ObservableProperty]
		string _MainStatus = Localization.Loc.StatusReady;
		[ObservableProperty]
		string _SecondaryStatus = Localization.Loc.MainWndTitle;

		#region Navigation Stuff
		static readonly NavTest navTest = new NavTest();
		static readonly NavTestUSB navTestUSB = new NavTestUSB();
		static readonly NavTestNet navTestNet = new NavTestNet();
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
		bool _NavTestUSBChecked = false;
		public bool NavTestUSBChecked
		{
			get => _NavTestUSBChecked;
			set
			{
				if (_NavTestUSBChecked = value) Content = navTestUSB;
				OnPropertyChanged(nameof(NavTestUSBChecked));
			}
		}
		bool _NavTestNetChecked = false;
		public bool NavTestNetChecked
		{
			get => _NavTestNetChecked;
			set
			{
				if (_NavTestNetChecked = value) Content = navTestNet;
				OnPropertyChanged(nameof(NavTestNetChecked));
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
				case "2": Content = navTestUSB; NavTestUSBChecked = true; break;
				case "3": Content = navTestNet; NavTestNetChecked = true; break;
				case "4": Content = navReview; NavReviewChecked = true; break;
				case "5": Content = navExport; NavExportChecked = true; break;
				case "O": Content = navSettings; NavSettingsChecked = true; break;
				default: throw new NotImplementedException();
			}
		}
		#endregion

		public HomeViewVM()
		{
			//Display login info in second status.
			Task.Run(() =>
			{
				Thread.Sleep(1000);
				SecondaryStatus = Localization.Loc.StatusLoggedIn.Replace("%User", UserName).Replace("%Type", UserType.ToString());
			});
			//Document.Create(container =>
			//{
			//	container.Page(page =>
			//	{
			//		page.Size(PageSizes.A4);
			//		page.Margin(2, Unit.Centimetre);
			//		page.PageColor(Colors.White);
			//		page.DefaultTextStyle(x => x.FontSize(20));

			//		page.Header().Text("标题").SemiBold().FontSize(40).FontColor(Colors.Red.Lighten1).FontFamily("FZLanTingHei-DB-GBK");

			//		page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
			//		{
			//			x.Spacing(20);

			//			x.Item().Text(Placeholders.LoremIpsum());
			//			x.Item().Image(Placeholders.Image(200, 100));
			//		});

			//		page.Footer().AlignCenter().Text(x =>
			//		{
			//			x.CurrentPageNumber().FontSize(10); x.Span("/").FontSize(10); x.TotalPages().FontSize(10);
			//		});
			//	});
			//}).ShowInPreviewerAsync();
		}
	}
}
