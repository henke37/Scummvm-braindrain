using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class UnknownEngineAccessor : BaseEngineAccessor {
		internal UnknownEngineAccessor(ScummVMConnector connector) : base(connector) {
		}

		internal override void LoadSymbols() {
		}
	}
}
