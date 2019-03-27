using System;
using System.Runtime.Serialization;

namespace DebugHelp {
	[Serializable]
	public class IncompleteReadException : Exception {

		internal IncompleteReadException() : base(Resources.ReadTooLitte) {
		}

		protected IncompleteReadException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}