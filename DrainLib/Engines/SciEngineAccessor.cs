using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class SciEngineAccessor : ADBaseEngineAccessor {
		public SciEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("Sci::SciEngine");
			var gameDescriptionOffset = Connector.resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(gameDescriptionOffset, true);
		}


	}
}
