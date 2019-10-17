using System;

namespace DrainLib.Engines {
	public class TuckerEngineAccessor : BaseEngineAccessor {
		#region Symbol data
		private int currentPartOffset;
		private int locationOffset;
		private int flagsTableOffset;
		private const int FlagCount = 300;
		#endregion

		public TuckerEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "tucker";

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Tucker::TuckerEngine");
			currentPartOffset = Resolver.FieldOffset(engineCl, "_currentPart");
			locationOffset = Resolver.FieldOffset(engineCl, "_location");
			flagsTableOffset = Resolver.FieldOffset(engineCl, "_flagsTable");
		}

		public TuckerState GetState() {
			var state = new TuckerState();
			state.CurrentPart = (TuckerState.PartEnum)MemoryReader.ReadUInt32(EngineAddr + currentPartOffset);
			state.Location = MemoryReader.ReadUInt32(EngineAddr + locationOffset);
			state.Flags = MemoryReader.ReadInt32Array(EngineAddr + flagsTableOffset, FlagCount);
			return state;
		}
	}

	public class TuckerState {
		public PartEnum CurrentPart;
		public uint Location;
		public int[] Flags;

		public enum PartEnum {
			Init,
			One,
			Two,
			Three
		}
	}

	
}
