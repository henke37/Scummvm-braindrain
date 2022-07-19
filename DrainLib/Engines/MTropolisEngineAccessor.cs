using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class MTropolisEngineAccessor : ADBaseEngineAccessor {
		public MTropolisEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("MTropolis::MTropolisEngine");

			LoadADSymbols(Connector.resolver.FieldOffset(engineCl, "_gameDescription"), true);
		}
	}
}
