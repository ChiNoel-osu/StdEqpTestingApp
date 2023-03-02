using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StdEqpTesting.Model
{
	public class ReportModel
	{
		public string User { get; set; }
		public UserTypeEnum UserType { get; set; }
		public DateTime DateTime { get; set; }
	}
}
