using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
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
		System.Windows.Controls.UserControl _Content = Nav1;
		[ObservableProperty]
		string _MainStatus = "Ready";

		#region Navigation Stuff
		static readonly Nav1 Nav1 = new Nav1();
		static readonly Nav2 Nav2 = new Nav2();
		bool _Nav1Checked = true;
		public bool Nav1Checked
		{
			get => _Nav1Checked;
			set
			{
				if (_Nav1Checked = value) Content = Nav1;
				OnPropertyChanged(nameof(Nav1Checked));
			}
		}
		bool _Nav2Checked = false;
		public bool Nav2Checked
		{
			get => _Nav2Checked;
			set
			{
				if (_Nav2Checked = value) Content = Nav2;
				OnPropertyChanged(nameof(Nav2Checked));
			}
		}
		[RelayCommand]
		public void TabNavigate(string TabNumber)
		{
			switch (TabNumber)
			{
				case "1": Content = Nav1; Nav1Checked = true; break;
				case "2": Content = Nav2; Nav2Checked = true; break;
				default: throw new NotImplementedException();
			}
		}
		#endregion

		public HomeViewVM()
		{
			Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(2, Unit.Centimetre);
					page.PageColor(Colors.White);
					page.DefaultTextStyle(x => x.FontSize(20));

					page.Header().Text("标题").SemiBold().FontSize(40).FontColor(Colors.Red.Lighten1).FontFamily("FZLanTingHei-DB-GBK");

					page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
					{
						x.Spacing(20);

						x.Item().Text(Placeholders.LoremIpsum());
						x.Item().Image(Placeholders.Image(200, 100));
					});

					page.Footer().AlignCenter().Text(x =>
					{
						x.CurrentPageNumber().FontSize(10); x.Span("/").FontSize(10); x.TotalPages().FontSize(10);
					});
				});
			}).ShowInPreviewerAsync();
		}
	}
}
