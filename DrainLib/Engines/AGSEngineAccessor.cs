using System;

namespace DrainLib.Engines {
	class AGSEngineAccessor : ADBaseEngineAccessor {
		public AGSEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("AGS::AGSEngine");
			var descOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descOffset, true);
		}
	}
}
