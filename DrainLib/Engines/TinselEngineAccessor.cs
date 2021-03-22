using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	class TinselEngineAccessor : ADBaseEngineAccessor {
		public TinselEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Tinsel::TinselEngine");
			var descOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descOffset, true);
		}
	}
}
