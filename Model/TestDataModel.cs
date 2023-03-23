using CommunityToolkit.Mvvm.ComponentModel;

namespace StdEqpTesting.Model
{
	public partial class TestDataModel : ObservableObject
	{
		[ObservableProperty]
		int _Index; //Observe this bc user could be removing stuff and index should update.

		public string Value { get; set; }
	}
}
