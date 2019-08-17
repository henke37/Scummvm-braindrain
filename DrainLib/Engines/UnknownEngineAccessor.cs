using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DrainLib.Engines {
	public class UnknownEngineAccessor : BaseEngineAccessor {
		internal UnknownEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			string mangledName = connector.rttiReader.GetMangledClassNameFromObjPtr(engineAddr);
			Debug.WriteLine($"Unknown engine class name \"{mangledName}\"");
		}

		public override string GameId => "???";

		internal override void LoadSymbols() {
		}
	}
}
