using System.IO.Ports;

namespace StdEqpTesting.Model
{
	public struct COMPortPropModel
	{
		public int BaudRate { get; set; }
		public Parity Parity { get; set; }
		public int DataBits { get; set; }
		public StopBits StopBits { get; set; }
		public Handshake Handshake { get; set; }
		public string EncodingString { get; set; }
		public int ReadTimeout { get; set; }
		public int WriteTimeout { get; set; }
	}
}
