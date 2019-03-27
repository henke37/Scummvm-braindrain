using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DebugHelp.RTTI {
	class ClassHierarchyDescriptor {
		public ClassHierarchyFlags Flags;
		public List<BaseClassDescriptor> BaseClasses;

		public class BaseClassDescriptor {
			public TypeDescriptor TypeDescriptor;
			public uint NumContainedBases;
			public PMD DisplacementData;

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			internal struct MemoryStruct {
				public uint pTypeDescriptor;
				public uint NumContainedBases;
				public PMD DisplacementData;
				public uint Flags;
			}
		}

		public struct PMD {
			int mdisp;
			int pdisp;
			int vdisp;

			public uint LocateBaseObject(uint completeObjectAddr, ProcessMemoryReader reader) {
				completeObjectAddr += (uint)mdisp;
				if(pdisp != -1) {
					uint vtbl = (uint)(completeObjectAddr + pdisp);
					completeObjectAddr += reader.ReadUInt32At((uint)(vtbl + vdisp));
				}
				return completeObjectAddr;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MemoryStruct {
			internal uint Signature;
			internal ClassHierarchyFlags Flags;
			internal uint numBaseClasses;
			internal uint pBaseClassArray;
		}

		[Flags]
		public enum ClassHierarchyFlags : uint {
			MultipleInhertience = 1,
			VirtualInhertience = 2
		}
	}
}
