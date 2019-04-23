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
		private ScummDiffer differ;

		public ScummResearchForm() {
			InitializeComponent();

			connector = new ScummVMConnector();

			differ = new ScummDiffer();
			differ.DifferenceFound += Differ_DifferenceFound;

			Update();
		}

		private void Differ_DifferenceFound(string varName, object oldVal, object newVal) {
			outputBox.AppendText($"{varName} {oldVal} -> {newVal}\n");
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

			if(engine != null && !engine.IsActiveEngine) engine = null;
			if(engine == null) {
				engine = connector.GetEngine();
			}
			if(engine == null) return;
		}

		private new void Update() {
			UpdateConnector();
			var scummEngine = engine as ScummEngineAccessor;
			if(scummEngine == null) return;

			var state = scummEngine.GetScummState();

			differ.diff(state);
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
