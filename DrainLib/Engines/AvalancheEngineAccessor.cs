using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	class AvalancheEngineAccessor : ADBaseEngineAccessor {
		#region Symbol data
		#endregion

		public AvalancheEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Avalanche::AvalancheEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			LoadADSymbols(descriptorOffset, true);
		}
	}
}
