using DebugHelp;
using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public abstract class BaseEngineAccessor {
		protected ScummVMConnector Connector;

		internal BaseEngineAccessor(ScummVMConnector connector) {
			this.Connector = connector;
		}

		bool IsActiveEngine {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
