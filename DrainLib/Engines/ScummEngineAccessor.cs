using System;
using System.Collections.Generic;
using DebugHelp;

namespace DrainLib.Engines {
	public class ScummEngineAccessor : BaseEngineAccessor {
		internal ScummEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			
		}
	}
}
