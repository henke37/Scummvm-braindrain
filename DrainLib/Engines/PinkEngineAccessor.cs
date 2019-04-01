using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class PinkEngineAccessor : ADBaseEngineAccessor {
		private uint moduleOffset;
		private uint objNameOffset;
		private uint pageOffset;
		private uint moduleVariablesOffset;
		private uint pageVariablesOffset;

		internal PinkEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineSymb = Connector.resolver.FindClass("Pink::PinkEngine");
			var descOffset = Connector.resolver.FieldOffset(engineSymb, "_desc");
			moduleOffset = Connector.resolver.FieldOffset(engineSymb, "_module");

			var namedObjSymb = Connector.resolver.FindClass("Pink::NamedObject");
			objNameOffset = Connector.resolver.FieldOffset(namedObjSymb, "_name");

			var moduleSymb = Connector.resolver.FindClass("Pink::Module");
			pageOffset = Connector.resolver.FieldOffset(moduleSymb,"_page");
			moduleVariablesOffset = Connector.resolver.FieldOffset(moduleSymb, "_variables");

			var pageSymb = Connector.resolver.FindClass("Pink::GamePage");
			pageVariablesOffset = Connector.resolver.FieldOffset(pageSymb,"_variables");

			LoadADSymbols(descOffset);
		}

		public PinkState GetPinkState() {
			var state = new PinkState();

			var modulePtrVal = Connector.memoryReader.ReadUInt32(EngineAddr + moduleOffset);

			var moduleNameAddr = modulePtrVal + objNameOffset;
			state.Module = ReadComString(moduleNameAddr);
			var pagePtrVal = Connector.memoryReader.ReadUInt32(modulePtrVal + pageOffset);
			var pageNameAddr = pagePtrVal + objNameOffset;
			state.Page = ReadComString(pageNameAddr);
			return state;
		}
	}

	[Serializable]
	public class PinkState {
		public string Module;
		public string Page;
	}
}
