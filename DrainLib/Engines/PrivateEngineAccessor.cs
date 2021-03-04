using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class PrivateEngineAccessor : ADBaseEngineAccessor {
		public PrivateEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Private::PrivateEngine");
			var descOffset=Resolver.FieldOffset(engineCl, "_gameDescription");

			LoadADSymbols(descOffset, true);
		}
	}
}
