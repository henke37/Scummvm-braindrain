using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class PlumbersEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		private int curSceneIdxOffset;
		#endregion

		public PlumbersEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "plumbers";

		internal override void LoadSymbols() {
			var engineCl=Connector.resolver.FindClass("Plumbers::PlumbersGame");
			curSceneIdxOffset = Connector.resolver.FieldOffset(engineCl, "_curSceneIdx");
		}

		public PlumbersState GetState() {
			var state = new PlumbersState();
			state.CurrentScene = Connector.memoryReader.ReadInt32(EngineAddr + curSceneIdxOffset);
			return state;
		}
	}

	public class PlumbersState {
		public int CurrentScene;
	}
}
