using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class UnknownEngineAccessor : BaseEngineAccessor {
		internal UnknownEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "???";

		internal override void LoadSymbols() {
		}
	}
}
