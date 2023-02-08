using CommunityToolkit.Mvvm.Input;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using StdEqpTesting.View;
using System;
using System.IO;
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
			return;

			Plot1.Annotations.Add(new LineAnnotation
			{
				StrokeThickness = 2,
				Color = OxyColors.Gray,
				Type = LineAnnotationType.Horizontal,
				LineStyle = OxyPlot.LineStyle.LongDashDot,
				Y = 0
			});
			Plot1.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, TicklineColor = OxyColors.White, TextColor = OxyColors.White });
			Plot1.Axes.Add(new LinearAxis { Position = AxisPosition.Left, TicklineColor = OxyColors.White, TextColor = OxyColors.White });
			Plot1.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.01, title: "BIG"));

			IWorkbook book = new XSSFWorkbook();
			ISheet sheet = book.CreateSheet("mother fuck");
			IRow row = sheet.CreateRow(0);
			ICell cell = row.CreateCell(0);
			ICellStyle style = book.CreateCellStyle();
			IFont font = book.CreateFont();
			cell.SetCellValue("All ur heads look bloody twelve feet tall!");
			style.FillForegroundColor = IndexedColors.DarkRed.Index;
			style.FillPattern = FillPattern.SolidForeground;
			style.BorderRight = BorderStyle.Medium;
			font.Color = IndexedColors.Orange.Index;
			style.SetFont(font);
			cell.CellStyle = style;
			sheet.SetColumnWidth(0, cell.StringCellValue.Length * 256);

			ICell cell2 = row.CreateCell(1);
			ICellStyle style2 = book.CreateCellStyle();
			cell2.SetCellValue("OK MATE, yoou suck");
			style2.FillForegroundColor = IndexedColors.BrightGreen.Index;
			style2.FillPattern = FillPattern.SolidForeground;
			cell2.CellStyle = style2;
			sheet.SetColumnWidth(1, cell2.StringCellValue.Length * 256);

			using (FileStream fs = File.Create("Excel.xlsx"))
			{
				book.Write(fs, false);
			}
		}
	}
}
