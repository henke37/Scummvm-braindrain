using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class SkyEngineAccessor : BaseEngineAccessor {
		internal SkyEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
		}
	}
}
