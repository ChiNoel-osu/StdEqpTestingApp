using AForge.Video;
using AForge.Video.DirectShow;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
//TODO: Logging
namespace StdEqpTesting.ViewModel
{
	public partial class NavTestImgVM : ObservableObject
	{
		public VideoCaptureDevice VCD;

		[ObservableProperty]
		BitmapSource _BitmapSource;
		[ObservableProperty]
		ObservableCollection<ComboBoxItem> _CameraList = new ObservableCollection<ComboBoxItem>();
		[ObservableProperty]
		bool _IsCameraRunning = false;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(StartCameraCommand))]
		ComboBoxItem _SelectedCamera;
		bool IsSelectedCamNotNull() => SelectedCamera is not null;

		void GetCamera()
		{
			// Create an instance of the FilterInfoCollection class
			FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			// Check if any video capture devices are available
			if (videoDevices.Count == 0)
				return;
			//Add found camera to ComboBox, use its Tag for FilterInfo.
			CameraList.Clear();
			foreach (FilterInfo filterInfo in videoDevices)
				CameraList.Add(new ComboBoxItem { Content = filterInfo.Name, Tag = filterInfo });
		}

		[RelayCommand(CanExecute = nameof(IsSelectedCamNotNull))]
		public void StartCamera()
		{
			if (IsCameraRunning)
			{
				VCD.SignalToStop();
				BitmapSource = null;
				while (VCD.IsRunning) ; //Wait for it to stop
			}
			else
			{
				// Select the first available video capture device
				FilterInfo videoDevice = (FilterInfo)SelectedCamera.Tag;
				// Create an instance of the VideoCaptureDevice class
				VCD = new VideoCaptureDevice(videoDevice.MonikerString);
				// Set the NewFrame event handler
				VCD.NewFrame += OnNewFrame;
				// Start the video capture device
				VCD.Start();
			}
			IsCameraRunning = VCD.IsRunning;
		}

		private void OnNewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{   // Convert the Bitmap to a BitmapSource
				BitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
					eventArgs.Frame.GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			});
		}

		public NavTestImgVM()
		{
			GetCamera();
		}
	}
}
