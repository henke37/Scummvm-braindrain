using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class ToonEngineAccessor : BaseEngineAccessor {
		internal ToonEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "toon";

		internal override void LoadSymbols() {
		}
	}
}
