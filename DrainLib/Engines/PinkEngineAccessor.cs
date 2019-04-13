using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace DrainLib.Engines {
	public class PinkEngineAccessor : ADBaseEngineAccessor {
		private uint moduleOffset;
		private uint gameVarsOffset;
		private uint objNameOffset;
		private uint pageOffset;
		private uint pagesOffset;
		private uint moduleVariablesOffset;
		private uint pageVariablesOffset;

		private uint stringMapStorageOffset;
		private uint stringMapMaskOffset;
		private const uint StringMapNodeDummyVal = 1;
		private uint stringMapNodeKeyOffset;
		private uint stringMapNodeValueOffset;
		private uint pagesArrOffset;
		private uint pagesArrSizeOffset;
		private uint pagesArrStorageOffset;

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

			var pagesSymb = Connector.resolver.FindField(moduleSymb, "_pages");
			pagesOffset = (uint)pagesSymb.offset;
			var pagesClSymb = pagesSymb.type;
			var pagesBaseClSymb = Connector.resolver.GetBaseClass(pagesClSymb);
			var pagesBaseName = pagesBaseClSymb.name;
			pagesArrOffset=(uint)pagesBaseClSymb.offset;
			pagesArrSizeOffset = Connector.resolver.FieldOffset(pagesBaseClSymb, "_size");
			pagesArrStorageOffset = Connector.resolver.FieldOffset(pagesBaseClSymb, "_storage");


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

			state.GameVars = ReadStringMap(EngineAddr + gameVarsOffset);
			state.ModuleVars = ReadStringMap(modulePtrVal + moduleVariablesOffset);
			state.Pages = ReadPages(modulePtrVal,out state.CurrentPage);

			return state;
		}

		private Dictionary<string,Page> ReadPages(uint moduleAddr, out Page currentPage) {
			var pagePtrs = ReadPagePointers(moduleAddr, out var pagesSize);
			var pageMap = new Dictionary<string, Page>((int)pagesSize);

			var currentPageAddr = Connector.memoryReader.ReadUInt32(moduleAddr + pageOffset);
			currentPage = null;

			foreach(var pagePtr in pagePtrs) {
				var pageNameAddr = pagePtr + objNameOffset;
				var page = new Page();
				page.Name = ReadComString(pageNameAddr);
				page.Vars = ReadStringMap(pagePtr + pageVariablesOffset);
				pageMap.Add(page.Name, page);
				if(pagePtr==currentPageAddr) {
					currentPage = page;
				}
			}

			return pageMap;
		}

		private UInt32[] ReadPagePointers(uint moduleAddr, out uint pagesSize) {
			var pagesArrAddr = moduleAddr + pagesOffset + pagesArrOffset;
			var pagesStorage = Connector.memoryReader.ReadUInt32(pagesArrAddr + pagesArrStorageOffset);
			pagesSize = Connector.memoryReader.ReadUInt32(pagesArrAddr + pagesArrSizeOffset);
			Debug.Assert(pagesStorage >= 4000);
			return Connector.memoryReader.ReadUInt32Array(pagesStorage, pagesSize);
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
		public Page CurrentPage;
		public Dictionary<string, string> GameVars;
		public Dictionary<string, string> ModuleVars;
		public Dictionary<string, Page> Pages;
	}

	public class Page {
		public string Name;
		public Dictionary<string, string> Vars;
	}
}
