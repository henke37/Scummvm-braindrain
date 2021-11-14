using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	class DirectorEngineAccessor : ADBaseEngineAccessor {
		#region Symbol data
		//engine class
		private int lingoOffset;
		private int stageOffset;
		#endregion

		#region Semistatic data
		private IntPtr lingoAddr;
		private IntPtr stageAddr;
		#endregion
		public DirectorEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
		}


		private void LoadSemiStaticData() {
			stageAddr = MemoryReader.ReadIntPtr(EngineAddr + stageOffset);
			lingoAddr = MemoryReader.ReadIntPtr(EngineAddr + lingoOffset);
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Director::DirectorEngine");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			stageOffset = Resolver.FieldOffset(engineCl, "_stage");
			lingoOffset = Resolver.FieldOffset(engineCl, "_lingo");

			LoadADSymbols(descriptorOffset, true);
		}
	}
}
