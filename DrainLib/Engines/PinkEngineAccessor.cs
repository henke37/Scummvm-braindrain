using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DrainLib.Engines {
	public class PinkEngineAccessor : ADBaseEngineAccessor {
		private uint moduleOffset;
		private uint gameVarsOffset;
		private uint objNameOffset;
		private uint pageOffset;
		private uint moduleVariablesOffset;
		private uint pageVariablesOffset;

		private uint stringMapStorageOffset;
		private uint stringMapMaskOffset;
		private const uint StringMapNodeDummyVal = 1;
		private uint stringMapNodeKeyOffset;
		private uint stringMapNodeValueOffset;

		internal PinkEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineSymb = Connector.resolver.FindClass("Pink::PinkEngine");
			var descOffset = Connector.resolver.FieldOffset(engineSymb, "_desc");
			moduleOffset = Connector.resolver.FieldOffset(engineSymb, "_module");
			gameVarsOffset = Connector.resolver.FieldOffset(engineSymb, "_variables");

			var namedObjSymb = Connector.resolver.FindClass("Pink::NamedObject");
			objNameOffset = Connector.resolver.FieldOffset(namedObjSymb, "_name");

			var moduleSymb = Connector.resolver.FindClass("Pink::Module");
			pageOffset = Connector.resolver.FieldOffset(moduleSymb,"_page");
			moduleVariablesOffset = Connector.resolver.FieldOffset(moduleSymb, "_variables");

			var pageSymb = Connector.resolver.FindClass("Pink::GamePage");
			pageVariablesOffset = Connector.resolver.FieldOffset(pageSymb,"_variables");

			LoadADSymbols(descOffset);

			LoadStringMapSymbols();
		}

		private void LoadStringMapSymbols() {
			var comStringMapClSymb = Connector.resolver.FindTypeDef("Common::StringMap");
			var hashMapSymb=comStringMapClSymb.type;

			stringMapStorageOffset = Connector.resolver.FieldOffset(hashMapSymb, "_storage");
			stringMapMaskOffset = Connector.resolver.FieldOffset(hashMapSymb, "_mask");

			var nodeClSymb = Connector.resolver.FindNestedClass(hashMapSymb, "Node");
			stringMapNodeKeyOffset = Connector.resolver.FieldOffset(nodeClSymb, "_key");
			stringMapNodeValueOffset = Connector.resolver.FieldOffset(nodeClSymb, "_value");
		}

		public PinkState GetPinkState() {
			var state = new PinkState();

			var modulePtrVal = Connector.memoryReader.ReadUInt32(EngineAddr + moduleOffset);

			var moduleNameAddr = modulePtrVal + objNameOffset;
			state.Module = ReadComString(moduleNameAddr);
			var pagePtrVal = Connector.memoryReader.ReadUInt32(modulePtrVal + pageOffset);
			var pageNameAddr = pagePtrVal + objNameOffset;
			state.Page = ReadComString(pageNameAddr);

			state.GameVars = ReadStringMap(EngineAddr + gameVarsOffset);
			state.ModuleVars=ReadStringMap(modulePtrVal + moduleVariablesOffset);
			state.PageVars = ReadStringMap(pagePtrVal + pageVariablesOffset);

			return state;
		}

		protected Dictionary<string,string> ReadStringMap(uint mapAddr) {
			var storagePtrVal = Connector.memoryReader.ReadUInt32(mapAddr + stringMapStorageOffset);
			var size = Connector.memoryReader.ReadUInt32(mapAddr + stringMapMaskOffset) + 1;

			Dictionary<string, string> mapContents = new Dictionary<string, string>();

			var nodePtrVals = Connector.memoryReader.ReadUInt32Array(storagePtrVal, size);
			foreach(var nodeAddr in nodePtrVals) {
				if(nodeAddr == 0 || nodeAddr == StringMapNodeDummyVal) continue;
				string key = ReadComString(nodeAddr + stringMapNodeKeyOffset);
				string value = ReadComString(nodeAddr + stringMapNodeValueOffset);
				mapContents.Add(key, value);
			}

			return mapContents;
		}
	}

	[Serializable]
	public class PinkState {
		public string Module;
		public string Page;
		public Dictionary<string, string> GameVars;
		public Dictionary<string, string> ModuleVars;
		public Dictionary<string, string> PageVars;
	}
}
