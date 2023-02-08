using CommunityToolkit.Mvvm.ComponentModel;
using StdEqpTesting.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StdEqpTesting.ViewModel
{
	public partial class MainViewVM : ObservableObject
	{
		public string UserName { get; set; }
		public UserTypeEnum UserType { get; set; }


		[ObservableProperty]
		short _RndNumber = 0;

		public MainViewVM()
		{
			Random random = new Random(DateTime.Now.Millisecond);
			Task.Run(() =>
			{
				while (true)
				{
					Thread.Sleep(100);
					RndNumber = (short)random.Next(short.MinValue, short.MaxValue);
				}
			});
		}
	}
}
