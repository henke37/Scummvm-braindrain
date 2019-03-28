using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class ToonEngineAccessor : BaseEngineAccessor {
		internal ToonEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
		}
	}
}
