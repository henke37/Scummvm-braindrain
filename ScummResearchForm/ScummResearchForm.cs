using DebugHelp;
using DrainLib;
using DrainLib.Engines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScummResearchForm {
	public partial class ScummResearchForm : Form {
		private ScummVMConnector connector;
		private BaseEngineAccessor engine;

		public ScummResearchForm() {
			InitializeComponent();

			connector = new ScummVMConnector();
			Update();
		}

		private void UpdateConnector() {
			if(!connector.Connected) {
				try {
					connector.Connect();
				} catch(ProcessNotFoundException) {
					return;
				} catch(IncompleteReadException) {
					return;
				}
			}

			if(!engine.IsActiveEngine) engine = null;
			if(engine == null) {
				engine = connector.GetEngine();
			}
			if(engine == null) return;
		}

		private new void Update() {
			UpdateConnector();
		}

		private void UpdateTimer_Tick(object sender, EventArgs e) {
			//turn off the timer while processing to prevent multiple processings run at the same time
			//also makes exceptions not turn it back on
			UpdateTimer.Enabled = false;
			Update();
			UpdateTimer.Enabled = true;
		}
	}
}
