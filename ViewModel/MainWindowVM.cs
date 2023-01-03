using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using StdEqpTesting.Model;
using StdEqpTesting.View;
using System;
using System.Windows;

namespace StdEqpTesting.ViewModel
{
	public partial class MainWindowVM
	{
		[RelayCommand]
		public void LoadNextWnd(Window parent)
		{
			LoginWindow loginWindow = new LoginWindow();
			parent.Close();
			loginWindow.Show();
		}

		public PlotModel Plot1 { get; set; } = new PlotModel()
		{
			Title = "DAmn",
			Subtitle = "piss",
			SubtitleFont = "Comic Sans MS",
			SubtitleFontSize = 20,
			PlotType = PlotType.Cartesian,
			TitleColor = OxyColors.White,
			SubtitleColor = OxyColors.Lime,
			PlotAreaBorderColor = OxyColors.White
		};

		public MainWindowVM()
		{
			Plot1.Annotations.Add(new LineAnnotation
			{
				StrokeThickness = 2,
				Color = OxyColors.Gray,
				Type = LineAnnotationType.Horizontal,
				LineStyle = LineStyle.LongDashDot,
				Y = 0
			});
			Plot1.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, TicklineColor = OxyColors.White, TextColor = OxyColors.White });
			Plot1.Axes.Add(new LinearAxis { Position = AxisPosition.Left, TicklineColor = OxyColors.White, TextColor = OxyColors.White });
			Plot1.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.01, title: "BIG"));
		}
	}
}
