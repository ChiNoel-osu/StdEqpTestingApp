using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace StdEqpTesting.Model
{
	public partial class TestTabItemModel : ObservableObject
	{
		[ObservableProperty]
		ObservableCollection<TestDataModel> _DataListBox = new ObservableCollection<TestDataModel>();

		public bool NoCOM { get; }

		public string Header { get; set; }

		public TestTabItemModel(bool hasCOM = true)
		{
			NoCOM = !hasCOM;
			DataListBox.Add(new TestDataModel() { Index = 0, Value = "AHHHHH" });
		}
	}
}
