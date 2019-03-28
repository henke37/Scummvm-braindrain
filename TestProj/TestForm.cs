using DrainLib;
using DrainLib.Engines;
using System;
using System.Windows.Forms;

namespace TestProj {
	public partial class TestForm : Form {
		private ScummVMConnector connector;

		private BaseEngineAccessor engine;

		public TestForm() {
			InitializeComponent();

			connector = new ScummVMConnector();
			Update();
		}

		private new void Update() {
			if(!connector.Connected) {
				try {
					connector.Connect();
				} catch (ProcessNotFoundException) {
					return;
				}
			}

			if(engine == null || !engine.IsActiveEngine) {
				engine = connector.GetEngine();
			}
			if(engine == null) return;
		}
	}
}
