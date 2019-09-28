using System;

namespace DrainLib.Engines {
	public class TuckerEngineAccessor : BaseEngineAccessor {
		#region Symbol data
		private int currentPartOffset;
		private int locationOffset;
		#endregion

		public TuckerEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "tucker";

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("Tucker::TuckerEngine");
			currentPartOffset = Connector.resolver.FieldOffset(engineCl, "_currentPart");
			locationOffset = Connector.resolver.FieldOffset(engineCl, "_location");
		}

		public TuckerState GetState() {
			var state = new TuckerState();
			state.CurrentPart = (TuckerState.PartEnum)Connector.memoryReader.ReadUInt32(EngineAddr + currentPartOffset);
			state.Location = Connector.memoryReader.ReadUInt32(EngineAddr + locationOffset);

			return state;
		}
	}

	public class TuckerState {
		public PartEnum CurrentPart;
		public uint Location;

		public enum PartEnum {
			Init,
			One,
			Two,
			Three
		}
	}

	
}
