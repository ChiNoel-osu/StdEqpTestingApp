﻿using StdEqpTesting.ViewModel;
using System.Windows;

namespace StdEqpTesting
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static StartupMainViewModel MainVM { get; } = new StartupMainViewModel();
		public MainWindow()
		{
			InitializeComponent();
			DataContext = MainVM;
		}
	}
}
