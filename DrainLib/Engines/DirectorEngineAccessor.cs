using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	class DirectorEngineAccessor : ADBaseEngineAccessor {
		public DirectorEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Director::DirectorEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descriptorOffset, true);
		}
	}
}
