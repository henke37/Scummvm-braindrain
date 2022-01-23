using System;

namespace DrainLib.Engines {
	public class MadeEngineAccessor : ADBaseEngineAccessor {
		public MadeEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Made::MadeEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descriptorOffset, true);
		}
	}
}
