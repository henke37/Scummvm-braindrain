using System;

namespace DrainLib.Engines {
	public class NeverhoodEngineAccessor : ADBaseEngineAccessor {

		#region Symbol data
		//engine class
		private int gameStateOffset;
		private int gameVarsOffset;
		#endregion

		#region Semistatic data
		private IntPtr gameVarsAddr;
		#endregion

		public NeverhoodEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
		}

		public override string GameId => "neverhood";

		private void LoadSemiStaticData() {
			gameVarsAddr = MemoryReader.ReadIntPtr(EngineAddr + gameVarsOffset);
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Neverhood::NeverhoodEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			gameStateOffset = Resolver.FieldOffset(engineCl, "_gameState");
			gameVarsOffset = Resolver.FieldOffset(engineCl, "_gameVars");
			LoadADSymbols(descriptorOffset, true);
		}
	}
}
