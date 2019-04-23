using DebugHelp;
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
			if(engine.MainMenuOpen) {
				statusTxt.Text = "Main menu open";
				return;
			}
			{
				var video = engine.GetVideoState();
				if(video!=null) {
					statusTxt.Text = $"{video.FileName} {video.CurrentFrame}/{video.FrameCount}";
					return;
				}
			}
			if(engine is ScummEngineAccessor scummEngine) {
			}
			if(engine is PinkEngineAccessor pinkEngine) {
				var state = pinkEngine.GetPinkState();
				statusTxt.Text = $"{state.Module}/{state.CurrentPage.Name}";
				return;
			}
			if(engine is QueenEngineAccessor queenEngine) {
				var state = queenEngine.GetState();
				statusTxt.Text = $"{state.CurrentRoom}";
				return;
			}
			if(engine is SkyEngineAccessor skyEngine) {
				var state = skyEngine.GetState();
			}
			if(engine is ToonEngineAccessor toonEngine) {
				var state = toonEngine.GetState();

				statusTxt.Text = $"{state.CurrentScene}";
				return;
			}
			if(engine is DrasculaEngineAccessor drasculaEngine) {
				var state = drasculaEngine.GetState();

				statusTxt.Text = $"{state.RoomNumber}";
				return;
			}
			statusTxt.Text = engine.GameId;
		}

		private void UpdateConnector() {
			if(!connector.Connected) {
				try {
					connector.Connect();
				} catch (ProcessNotFoundException) {
					return;
				} catch (IncompleteReadException) {
					return;
				}
			}

			if(engine != null && !engine.IsActiveEngine) engine = null;
			if(engine == null) {
				engine = connector.GetEngine();
			}
			if(engine == null) return;
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
