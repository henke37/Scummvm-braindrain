using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using static DrainLib.Engines.PinkState;

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
			var engineSymb = Resolver.FindClass("Pink::PinkEngine");
			var descOffset = Resolver.FieldOffset(engineSymb, "_desc");
			moduleOffset = Resolver.FieldOffset(engineSymb, "_module");
			gameVarsOffset = Resolver.FieldOffset(engineSymb, "_variables");

			var namedObjSymb = Resolver.FindClass("Pink::NamedObject");
			objNameOffset = Resolver.FieldOffset(namedObjSymb, "_name");

			var moduleSymb = Resolver.FindClass("Pink::Module");
			pageOffset = Resolver.FieldOffset(moduleSymb,"_page");
			moduleVariablesOffset = Resolver.FieldOffset(moduleSymb, "_variables");

			var pagesSymb = Resolver.FindField(moduleSymb, "_pages");
			pagesOffset = pagesSymb.offset;
			var pagesClSymb = pagesSymb.type;
			var pagesBaseClSymb = Resolver.GetBaseClass(pagesClSymb);
			var pagesBaseName = pagesBaseClSymb.name;
			pagesArrOffset=pagesBaseClSymb.offset;
			pagesArrSizeOffset = Resolver.FieldOffset(pagesBaseClSymb, "_size");
			pagesArrStorageOffset = Resolver.FieldOffset(pagesBaseClSymb, "_storage");


			var pageSymb = Resolver.FindClass("Pink::GamePage");
			pageVariablesOffset = Resolver.FieldOffset(pageSymb,"_variables");

			LoadADSymbols(descOffset, false);

			LoadStringMapSymbols();
		}

		private void LoadStringMapSymbols() {
			var comStringMapClSymb = Resolver.FindTypeDef("Common::StringMap");
			var hashMapSymb=comStringMapClSymb.type;

			stringMapStorageOffset = Resolver.FieldOffset(hashMapSymb, "_storage");
			stringMapMaskOffset = Resolver.FieldOffset(hashMapSymb, "_mask");

			var nodeClSymb = Resolver.FindNestedClass(hashMapSymb, "Node");
			stringMapNodeKeyOffset = Resolver.FieldOffset(nodeClSymb, "_key");
			stringMapNodeValueOffset = Resolver.FieldOffset(nodeClSymb, "_value");
		}

		public PinkState GetPinkState() {
			var state = new PinkState();

			var modulePtrVal = MemoryReader.ReadIntPtr(EngineAddr + moduleOffset);

			var moduleNameAddr = modulePtrVal + objNameOffset;
			state.Module = ReadComString(moduleNameAddr);
			var pagePtrVal = MemoryReader.ReadUInt32(modulePtrVal + pageOffset);

			state.GameVars = ReadStringMap(EngineAddr + gameVarsOffset);
			state.ModuleVars = ReadStringMap(modulePtrVal + moduleVariablesOffset);
			state.Pages = ReadPages(modulePtrVal,out state.CurrentPage);

			return state;
		}

		private Dictionary<string,Page> ReadPages(IntPtr moduleAddr, out Page currentPage) {
			var pagePtrs = ReadPagePointers(moduleAddr, out var pagesSize);
			var pageMap = new Dictionary<string, Page>((int)pagesSize);

			var currentPageAddr = MemoryReader.ReadIntPtr(moduleAddr + pageOffset);
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
			var pagesStorage = MemoryReader.ReadIntPtr(pagesArrAddr + pagesArrStorageOffset);
			pagesSize = MemoryReader.ReadUInt32(pagesArrAddr + pagesArrSizeOffset);
			Debug.Assert((int)pagesStorage >= 4000);
			return MemoryReader.ReadIntPtrArray(pagesStorage, pagesSize);
		}

		protected Dictionary<string,string> ReadStringMap(IntPtr mapAddr) {
			var storagePtrVal = MemoryReader.ReadIntPtr(mapAddr + stringMapStorageOffset);
			var size = MemoryReader.ReadUInt32(mapAddr + stringMapMaskOffset) + 1;

			Dictionary<string, string> mapContents = new Dictionary<string, string>();

			var nodePtrVals = MemoryReader.ReadIntPtrArray(storagePtrVal, size);
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

		public class Page {
			public string Name;
			public Dictionary<string, string> Vars;
		}
	}

	
}
