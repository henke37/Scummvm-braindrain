using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace DrainLib.Engines {
	public class PinkEngineAccessor : ADBaseEngineAccessor {

		#region Symbol data
		private int moduleOffset;
		private int gameVarsOffset;
		private int objNameOffset;
		private int pageOffset;
		private int pagesOffset;
		private int moduleVariablesOffset;
		private int pageVariablesOffset;

		private int stringMapStorageOffset;
		private int stringMapMaskOffset;
		private readonly IntPtr StringMapNodeDummyVal = (IntPtr)1;
		private int stringMapNodeKeyOffset;
		private int stringMapNodeValueOffset;
		private int pagesArrOffset;
		private int pagesArrSizeOffset;
		private int pagesArrStorageOffset;
		#endregion

		internal PinkEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
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
			pagesOffset = pagesSymb.offset;
			var pagesClSymb = pagesSymb.type;
			var pagesBaseClSymb = Connector.resolver.GetBaseClass(pagesClSymb);
			var pagesBaseName = pagesBaseClSymb.name;
			pagesArrOffset=pagesBaseClSymb.offset;
			pagesArrSizeOffset = Connector.resolver.FieldOffset(pagesBaseClSymb, "_size");
			pagesArrStorageOffset = Connector.resolver.FieldOffset(pagesBaseClSymb, "_storage");


			var pageSymb = Connector.resolver.FindClass("Pink::GamePage");
			pageVariablesOffset = Connector.resolver.FieldOffset(pageSymb,"_variables");

			LoadADSymbols(descOffset, false);

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

			var modulePtrVal = Connector.memoryReader.ReadIntPtr(EngineAddr + moduleOffset);

			var moduleNameAddr = modulePtrVal + objNameOffset;
			state.Module = ReadComString(moduleNameAddr);
			var pagePtrVal = Connector.memoryReader.ReadUInt32(modulePtrVal + pageOffset);

			state.GameVars = ReadStringMap(EngineAddr + gameVarsOffset);
			state.ModuleVars = ReadStringMap(modulePtrVal + moduleVariablesOffset);
			state.Pages = ReadPages(modulePtrVal,out state.CurrentPage);

			return state;
		}

		private Dictionary<string,Page> ReadPages(IntPtr moduleAddr, out Page currentPage) {
			var pagePtrs = ReadPagePointers(moduleAddr, out var pagesSize);
			var pageMap = new Dictionary<string, Page>((int)pagesSize);

			var currentPageAddr = Connector.memoryReader.ReadIntPtr(moduleAddr + pageOffset);
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

		private IntPtr[] ReadPagePointers(IntPtr moduleAddr, out uint pagesSize) {
			var pagesArrAddr = moduleAddr + pagesOffset + pagesArrOffset;
			var pagesStorage = Connector.memoryReader.ReadIntPtr(pagesArrAddr + pagesArrStorageOffset);
			pagesSize = Connector.memoryReader.ReadUInt32(pagesArrAddr + pagesArrSizeOffset);
			Debug.Assert((int)pagesStorage >= 4000);
			return Connector.memoryReader.ReadIntPtrArray(pagesStorage, pagesSize);
		}

		protected Dictionary<string,string> ReadStringMap(IntPtr mapAddr) {
			var storagePtrVal = Connector.memoryReader.ReadIntPtr(mapAddr + stringMapStorageOffset);
			var size = Connector.memoryReader.ReadUInt32(mapAddr + stringMapMaskOffset) + 1;

			Dictionary<string, string> mapContents = new Dictionary<string, string>();

			var nodePtrVals = Connector.memoryReader.ReadIntPtrArray(storagePtrVal, size);
			foreach(var nodeAddr in nodePtrVals) {
				if(nodeAddr == IntPtr.Zero || nodeAddr == StringMapNodeDummyVal) continue;
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
