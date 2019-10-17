using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class SherlockEngineAccessor : ADBaseEngineAccessor {
		public SherlockEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Sherlock::SherlockEngine");
			var gameDescriptionOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			this.LoadADSymbols(gameDescriptionOffset,true);
		}
	}
}
