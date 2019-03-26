using System;
using System.Collections.Generic;

namespace DebugHelp.RTTI {
	class TypeDescriptor {
		public string MangledName;

		public TypeDescriptor(string mangledName) {
			MangledName = mangledName;
		}
	}
}
