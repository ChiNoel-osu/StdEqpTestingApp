using System;

namespace StdEqpTesting.Model
{
	public struct ReportModel
	{
		public string User { get; set; }
		public UserTypeEnum UserType { get; set; }
		public DateTime DateTime { get; set; }
	}
}
