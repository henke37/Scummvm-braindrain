using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class SkyEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		private int logicOffset;
		private int scriptVarsOffset;
		private const uint numScriptVars = 838;
		#endregion

		internal SkyEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "sky";

		internal override void LoadSymbols() {
			var engineClSymb = Connector.resolver.FindClass("Sky::SkyEngine");
			logicOffset = Connector.resolver.FieldOffset(engineClSymb, "_logic");

			var logicClSymb = Connector.resolver.FindClass("Sky::Logic");
			scriptVarsOffset = Connector.resolver.FieldOffset(logicClSymb, "_scriptVariables");
		}

		public SkyState GetState() {
			var logicPtrVal = Connector.memoryReader.ReadIntPtr(EngineAddr + logicOffset);

			var state = new SkyState();

			state.Vars = Connector.memoryReader.ReadUInt32Array(logicPtrVal + scriptVarsOffset, numScriptVars);

			return state;
		}

		public class SkyState {
			public uint[] Vars;
		}
	}
}
