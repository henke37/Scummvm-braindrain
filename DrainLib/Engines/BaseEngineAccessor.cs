using DebugHelp;
using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public abstract class BaseEngineAccessor {
		protected ScummVMConnector Connector;

		protected uint EngineAddr;

		internal BaseEngineAccessor(ScummVMConnector connector, uint engineAddr) {
			this.Connector = connector;
			this.EngineAddr = engineAddr;

			LoadBaseSymbols();
			LoadSymbols();
		}

		private void LoadBaseSymbols() {
			throw new NotImplementedException();
		}

		internal abstract void LoadSymbols();

		bool IsActiveEngine {
			get {
				var liveEnginePtrVal=Connector.memoryReader.ReadUInt32At(Connector.g_engineAddr);
				return liveEnginePtrVal == EngineAddr;
			}
		}
	}
}
