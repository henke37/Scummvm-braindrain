using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class HyperspaceDeliveryBoyEngineAccessor : BaseEngineAccessor {
		public HyperspaceDeliveryBoyEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "hbd";

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("HDB::HDBGame");
		}
	}
}
