﻿using System;
using DrainLib.Engines;

namespace DrainLib {
	internal class TestBedEngineAccessor : BaseEngineAccessor {
		public TestBedEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "testbed";

		internal override void LoadSymbols() {
		}
	}
}