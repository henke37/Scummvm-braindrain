using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class SagaEngineAccessor : ADBaseEngineAccessor {
		public SagaEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Saga::SagaEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descriptorOffset,true);
		}
	}
}
