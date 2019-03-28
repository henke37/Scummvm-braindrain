using DrainLib;
using DrainLib.Engines;
using System;
using System.Windows.Forms;
using TestProj.Properties;

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
			UpdateConnector();
			if(!connector.Connected) {
				statusTxt.Text = Resources.Status_NotConnected;
				return;
			}
			if(engine==null) {
				statusTxt.Text = Resources.Status_NoGame;
				return;
			}
			statusTxt.Text = engine.ToString();
		}

		private void UpdateConnector() {
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

		private void UpdateTimer_Tick(object sender, EventArgs e) {
			Update();
		}
	}
}
