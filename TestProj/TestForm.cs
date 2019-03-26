using DrainLib;
using System;
using System.Windows.Forms;

namespace TestProj {
	public partial class TestForm : Form {
		private ScummVMConnector connector;

		public TestForm() {
			InitializeComponent();

			connector = new ScummVMConnector();
			connector.Connect();

			var engine = connector.Engine;
		}
	}
}
