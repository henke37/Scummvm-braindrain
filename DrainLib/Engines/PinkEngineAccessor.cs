using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class PinkEngineAccessor : BaseEngineAccessor {
		internal PinkEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			
		}
	}
}
