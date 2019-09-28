using System;

namespace DrainLib.Engines {
	public class TeenAgentEngineAccessor : BaseEngineAccessor {
		public TeenAgentEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "teenAgent";

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("TeenAgent::TeenAgentEngine");
		}
	}
}
