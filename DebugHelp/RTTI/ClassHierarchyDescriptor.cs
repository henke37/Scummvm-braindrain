using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DebugHelp.RTTI {
	public class ClassHierarchyDescriptor {
		public ClassHierarchyFlags Flags;
		public List<BaseClassDescriptor> BaseClasses;

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
