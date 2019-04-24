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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScummResearchForm {
	public partial class ScummResearchForm : Form {
		private ScummVMConnector connector;
		private BaseEngineAccessor engine;
		private ScummDiffer differ;

		private static readonly Regex scummVarMatcher = new Regex(@"scummvar(\d+)", RegexOptions.Compiled);
		private static readonly Regex bitVarMatcher = new Regex(@"bitvar(\d+)", RegexOptions.Compiled);

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

		private void AddIgnore_Click(object sender, EventArgs e) {
			var form = new AddIgnoreForm();
			form.ShowDialog(this);
		}

		internal void AddIgnore(string varName) {
			IgnoreListBox.Items.Add(varName,true);
			RebuildIgnoreLists();
		}

		private void RebuildIgnoreLists() {
			differ.IgnoredVars.Clear();
			differ.IgnoredBitVars.Clear();

			foreach(string item in IgnoreListBox.CheckedItems) {
				Match m;

				m = scummVarMatcher.Match(item);
				if(m.Success) {
					int varIndex = int.Parse(m.Groups[1].Value);
					differ.IgnoredVars.Add(varIndex, true);
					continue;
				}
				m = bitVarMatcher.Match(item);
				if(m.Success) {
					int varIndex = int.Parse(m.Groups[1].Value);
					differ.IgnoredBitVars.Add(varIndex, true);
					continue;
				}
			}
		}
	}
}
