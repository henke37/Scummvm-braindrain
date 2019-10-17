﻿using System;
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
			var engineClSymb = Resolver.FindClass("Sky::SkyEngine");
			logicOffset = Resolver.FieldOffset(engineClSymb, "_logic");

			var logicClSymb = Resolver.FindClass("Sky::Logic");
			scriptVarsOffset = Resolver.FieldOffset(logicClSymb, "_scriptVariables");
		}

		public SkyState GetState() {
			var logicPtrVal = MemoryReader.ReadIntPtr(EngineAddr + logicOffset);

			var state = new SkyState();

			state.Vars = MemoryReader.ReadUInt32Array(logicPtrVal + scriptVarsOffset, numScriptVars);

			return state;
		}

		public class SkyState {
			public uint[] Vars;
		}
	}
}
