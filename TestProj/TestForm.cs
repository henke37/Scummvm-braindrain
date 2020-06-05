using Henke37.DebugHelp;
using DrainLib;
using DrainLib.Engines;
using System;
using System.Windows.Forms;
using TestProj.Properties;
using System.Text;

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
					var sb = new StringBuilder();
					if(!string.IsNullOrEmpty(video.FileName)) {
						sb.Append(video.FileName);
						sb.Append(' ');
					}
					if(video.FrameRate.Denominator != 0) {
						sb.AppendFormat("{0}/{1}",video.PlaybackPosition,video.Length);
					} else if(video.FrameCount!=0) {
						sb.AppendFormat("{0}/{1}", video.CurrentFrame, video.FrameCount);
					}
					if(sb.Length == 0) {
						statusTxt.Text = "Video";
					} else {
						statusTxt.Text = sb.ToString();
					}
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
			if(engine is TuckerEngineAccessor tuckerEngine) {
				var state = tuckerEngine.GetState();

				statusTxt.Text = $"{state.CurrentPart}/{state.Location}";
				return;
			}
			if(engine is HyperspaceDeliveryBoyEngineAccessor hdbEngine) {
				var state = hdbEngine.GetState();

				switch(state.State) {
					case HyperspaceDeliveryBoyEngineAccessor.GameState.Title:
						statusTxt.Text = "Title";
						return;
					case HyperspaceDeliveryBoyEngineAccessor.GameState.Menu:
						statusTxt.Text = "Menu";
						return;
					case HyperspaceDeliveryBoyEngineAccessor.GameState.Loading:
						break;

					case HyperspaceDeliveryBoyEngineAccessor.GameState.Play:
						statusTxt.Text = state.CurrentMap;
						return;
				}
			}
			if(engine is PlumbersEngineAccessor plumbersEngine) {
				var state = plumbersEngine.GetState();
				statusTxt.Text = $"{state.CurrentScene}/{state.CurrentBitmapIndex}";
				return;
			}
			if(engine is VoyeurEngineAccessor voyeurEngine) {
				var state = voyeurEngine.GetState();
				statusTxt.Text = $"{state.GameHour:D2}:{state.GameMinute:D2} {state.RTVCharge*100} %";
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
