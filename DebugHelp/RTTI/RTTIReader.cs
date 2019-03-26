using System;
using System.Collections.Generic;

namespace DebugHelp.RTTI {
	public class RTTIReader {
		private IProcessMemoryReader processMemoryReader;

		private Dictionary<uint, CompleteObjectLocator> completeObjectLocatorMap;

		public RTTIReader(IProcessMemoryReader processMemoryReader) {
			this.processMemoryReader = processMemoryReader ?? throw new ArgumentNullException(nameof(processMemoryReader));

			completeObjectLocatorMap = new Dictionary<uint, CompleteObjectLocator>();
		}

		private CompleteObjectLocator readObjPtr(uint objAddr) {
			var vtblPtrVal = processMemoryReader.ReadUInt32At(objAddr);
			var metaPtrVal = processMemoryReader.ReadUInt32At(vtblPtrVal - 4);

			if(completeObjectLocatorMap.TryGetValue(metaPtrVal, out CompleteObjectLocator objectLocator)) {
				return objectLocator;
			}

			throw new NotImplementedException();
		}

		public string GetMangledClassNameFromObjPtr(uint objAddr) {
			readObjPtr(objAddr);
			throw new NotImplementedException();
		}

		public bool IsObjectInheritingMangledType(uint objAddr, string mangledName) {
			readObjPtr(objAddr);
			throw new NotImplementedException();
		}
	}
}
