using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class PinkEngineAccessor : ADBaseEngineAccessor {
		internal PinkEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineSymb = Connector.resolver.FindClass("Pink::PinkEngine");
			var descOffset = Connector.resolver.FieldOffset(engineSymb, "_desc");

			LoadADSymbols(descOffset);
		}
	}
}
