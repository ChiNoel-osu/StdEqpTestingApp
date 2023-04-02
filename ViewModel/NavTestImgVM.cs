using AForge.Video;
using AForge.Video.DirectShow;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace StdEqpTesting.ViewModel
{
	public partial class NavTestImgVM : ObservableObject
	{
		public VideoCaptureDevice VCD;

		public int SelectedCapIndex { get; set; }

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(SaveImageCommand))]
		BitmapSource? _BitmapSource;
		[ObservableProperty]
		ObservableCollection<ComboBoxItem> _CameraList = new ObservableCollection<ComboBoxItem>();
		[ObservableProperty]
		ObservableCollection<string> _CameraCapList = new ObservableCollection<string>();
		[ObservableProperty]
		bool _IsCameraRunning = false;
		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(SaveImageCommand))]
		string _SavingFileName = string.Empty;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(StartCameraCommand))]
		ComboBoxItem? _SelectedCamera;
		partial void OnSelectedCameraChanged(ComboBoxItem? value)
		{   //Update VCD and capability list.
			if (value is null) return;
			VCD = new VideoCaptureDevice(((FilterInfo)value.Tag).MonikerString);
			CameraCapList.Clear();
			foreach (VideoCapabilities cap in VCD.VideoCapabilities)
				CameraCapList.Add($"{cap.FrameSize.Width}*{cap.FrameSize.Height}@{cap.AverageFrameRate}FPS");
		}
		bool IsSelectedCamNotNull() => SelectedCamera is not null;

		[RelayCommand]
		public void GetCamera()
		{
			FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			if (videoDevices.Count == 0)    //Check if any video capture devices are available
				return;
			//Add found camera to ComboBox, use its Tag for FilterInfo.
			CameraList.Clear();
			foreach (FilterInfo filterInfo in videoDevices)
				CameraList.Add(new ComboBoxItem { Content = filterInfo.Name, Tag = filterInfo });
			MainViewModel.MainVM?.UpdateMainStatus($"Got {videoDevices.Count} video devices", true);
		}

		[RelayCommand(CanExecute = nameof(IsSelectedCamNotNull))]
		public void StartCamera()
		{
			if (IsCameraRunning)
			{
				App.Logger.Info($"Stopping video device {VCD.Source}");
				VCD.SignalToStop();
				BitmapSource = null;
				while (VCD.IsRunning) ; //Wait for it to stop
			}
			else
			{
				if (SelectedCapIndex == -1) SelectedCapIndex = 0;
				App.Logger.Info($"Starting video device with MonikerString {VCD.Source}\nCapability: {CameraCapList[SelectedCapIndex]}");
				VCD.VideoResolution = VCD.VideoCapabilities[SelectedCapIndex];
				VCD.NewFrame += OnNewFrame;
				VCD.Start();
			}
			IsCameraRunning = VCD.IsRunning;
		}
		private void OnNewFrame(object sender, NewFrameEventArgs eventArgs)
		{   //This will be called like 30 times a sec i dunno if this is good.
			Application.Current.Dispatcher.Invoke(() =>
			{   // Convert the Bitmap to a BitmapSource
				this.BitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
					eventArgs.Frame.GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			});
		}

		bool CanSaveImage() => !(this.BitmapSource is null || string.IsNullOrWhiteSpace(SavingFileName));
		[RelayCommand(CanExecute = nameof(CanSaveImage))]
		public void SaveImage()
		{
			try
			{
				Directory.CreateDirectory(Properties.Settings.Default.ImageSaveDir);
				using FileStream fileStream = new FileStream(Path.Combine(Properties.Settings.Default.ImageSaveDir, SavingFileName + ".jpg"), FileMode.CreateNew);
				JpegBitmapEncoder encoder = new JpegBitmapEncoder();
				encoder.QualityLevel = Properties.Settings.Default.ImageSaveQuality;
				encoder.Frames.Add(BitmapFrame.Create(this.BitmapSource));
				encoder.Save(fileStream);
			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.Message, Localization.Loc.SaveImgFailed, MessageBoxButton.OK);
			}
		}
		[RelayCommand]
		public void TimeAsName() => SavingFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss");
		[RelayCommand]
		public void OpenImgLocation() => Process.Start("explorer.exe", Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.ImageSaveDir));

		public NavTestImgVM()
		{
			GetCamera();
		}
	}
}
