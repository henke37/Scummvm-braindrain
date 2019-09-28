using System;

namespace DrainLib.Engines {
	public class TuckerEngineAccessor : BaseEngineAccessor {
		public TuckerEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "tucker";

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("Tucker::TuckerEngine");
		}
	}
}
