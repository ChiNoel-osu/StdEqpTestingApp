using CommunityToolkit.Mvvm.ComponentModel;
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
		System.Windows.Controls.UserControl _Content = Nav1;
		[ObservableProperty]
		short _RndNumber = 0;

		#region Navigation Stuff
		static readonly Nav1 Nav1 = new Nav1();
		static readonly Nav2 Nav2 = new Nav2();
		bool _Nav1Checked = true;
		public bool Nav1Checked
		{
			get => _Nav1Checked;
			set
			{ if (_Nav1Checked = value) Content = Nav1; }
		}
		bool _Nav2Checked = false;
		public bool Nav2Checked
		{
			get => _Nav2Checked;
			set
			{ if (_Nav2Checked = value) Content = Nav2; }
		}
		#endregion

		public HomeViewVM()
		{
			Random random = new Random(DateTime.Now.Millisecond);
			Task.Run(() =>
			{
				while (true)
				{
					Thread.Sleep(1000);
					RndNumber = (short)random.Next(short.MinValue, short.MaxValue);
				}
			});
		}
	}
}
