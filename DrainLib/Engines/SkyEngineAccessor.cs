using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class SkyEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		private int logicOffset;
		private int scriptVarsOffset;
		private const uint numScriptVars = 838;
		#endregion

		private IntPtr logicPtrVal;

		internal SkyEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
		}

		private void LoadSemiStaticData() {
			logicPtrVal = MemoryReader.ReadIntPtr(EngineAddr + logicOffset);
			if(logicPtrVal == IntPtr.Zero) throw new InconsistentDataException();
		}

		public override string GameId => "sky";

		internal override void LoadSymbols() {
			var engineClSymb = Resolver.FindClass("Sky::SkyEngine");
			logicOffset = Resolver.FieldOffset(engineClSymb, "_skyLogic");

			var logicClSymb = Resolver.FindClass("Sky::Logic");
			scriptVarsOffset = Resolver.FieldOffset(logicClSymb, "_scriptVariables");
		}

		public SkyState GetState() {
			var state = new SkyState();

			state.Vars = MemoryReader.ReadUInt32Array(logicPtrVal + scriptVarsOffset, numScriptVars);

			return state;
		}

		public class SkyState {
			public uint[] Vars;
		}
	}
}
