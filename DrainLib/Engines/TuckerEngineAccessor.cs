using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
