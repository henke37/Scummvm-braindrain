using DebugHelp;
using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public abstract class BaseEngineAccessor {
		protected ScummVMConnector Connector;

		internal BaseEngineAccessor(ScummVMConnector connector) {
			this.Connector = connector;

			LoadBaseSymbols();
			LoadSymbols();
		}

		private void LoadBaseSymbols() {
			throw new NotImplementedException();
		}

		internal abstract void LoadSymbols();

		bool IsActiveEngine {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
