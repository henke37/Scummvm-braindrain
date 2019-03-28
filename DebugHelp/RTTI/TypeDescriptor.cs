using System;
using System.Collections.Generic;

namespace DebugHelp.RTTI {
	public class TypeDescriptor {
		public string MangledName;

		public TypeDescriptor(string mangledName) {
			MangledName = mangledName;
		}

		public override string ToString() {
			return MangledName;
		}
	}
}
