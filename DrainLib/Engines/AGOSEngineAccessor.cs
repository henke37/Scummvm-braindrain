using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class AGOSEngineAccessor : ADBaseEngineAccessor {
		public AGOSEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("AGOS::AGOSEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descriptorOffset, true);
		}
	}
}
