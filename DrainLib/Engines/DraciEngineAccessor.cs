using System;

namespace DrainLib.Engines {
	class DraciEngineAccessor : BaseEngineAccessor {
		public DraciEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "draci";

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Draci::DraciEngine");

		}
	}
}
