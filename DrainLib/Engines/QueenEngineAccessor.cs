using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class QueenEngineAccessor : BaseEngineAccessor {
		internal QueenEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "queen";

		internal override void LoadSymbols() {
			
		}
	}
}
