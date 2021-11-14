using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	class HypnoEngineAccessor : ADBaseEngineAccessor {
		public HypnoEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Hypno::HypnoEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descriptorOffset, true);
		}
	}
}
