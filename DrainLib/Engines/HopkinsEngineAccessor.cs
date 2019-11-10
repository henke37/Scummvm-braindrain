using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class HopkinsEngineAccessor : BaseEngineAccessor {
		public HopkinsEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "hopkins";

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Hopkins::HopkinsEngine");
		}
	}
}
