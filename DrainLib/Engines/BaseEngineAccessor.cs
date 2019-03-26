using DebugHelp;
using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public abstract class BaseEngineAccessor {
		private SymbolResolver resolver;

		internal BaseEngineAccessor(SymbolResolver resolver) {
			this.resolver = resolver;
		}
	}
}
