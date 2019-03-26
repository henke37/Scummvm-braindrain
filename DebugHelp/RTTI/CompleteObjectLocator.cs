using System;
using System.Collections.Generic;

namespace DebugHelp.RTTI {
	internal class CompleteObjectLocator {
		public uint signature;
		public uint objectRootOffset;
		public uint classDescriptorOffset;
	}
}
