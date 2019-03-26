using System;
using System.Runtime.Serialization;

namespace DebugHelp {
	[Serializable]
	public class IncompleteReadException : Exception {

		internal IncompleteReadException(string message) : base(message) {
		}

		internal IncompleteReadException(string message, Exception innerException) : base(message, innerException) {
		}

		protected IncompleteReadException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}