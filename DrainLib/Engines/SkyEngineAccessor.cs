using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class SkyEngineAccessor : BaseEngineAccessor {
		internal SkyEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "sky";

		internal override void LoadSymbols() {
		}
	}
}
