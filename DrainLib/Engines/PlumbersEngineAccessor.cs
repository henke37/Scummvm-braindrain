using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class PlumbersEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		private int curSceneIdxOffset;
		private int prevSceneIdxOffset;
		private int curBitmapIdxOffset;
		#endregion

		public PlumbersEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "plumbers";

		internal override void LoadSymbols() {
			var engineCl=Resolver.FindClass("Plumbers::PlumbersGame");
			curSceneIdxOffset = Resolver.FieldOffset(engineCl, "_curSceneIdx");
			prevSceneIdxOffset = Resolver.FieldOffset(engineCl, "_prvSceneIdx");
			curBitmapIdxOffset = Resolver.FieldOffset(engineCl, "_curBitmapIdx");
		}

		public PlumbersState GetState() {
			var state = new PlumbersState();
			state.CurrentScene = MemoryReader.ReadInt32(EngineAddr + curSceneIdxOffset);
			state.PrevScene = MemoryReader.ReadInt32(EngineAddr + prevSceneIdxOffset);
			state.CurrentBitmapIndex = MemoryReader.ReadInt32(EngineAddr + curBitmapIdxOffset);
			return state;
		}
	}

	public class PlumbersState {
		public int CurrentScene;
		public int PrevScene;
		public int CurrentBitmapIndex;
	}
}
