using System;
using System.Runtime.Serialization;

namespace DebugHelp {
	[Serializable]
	public class IncompleteReadException : Exception {

		public const int ErrorNumber = 299;

		public IncompleteReadException() : base(Resources.ReadTooLitte) {
		}
		public IncompleteReadException(Exception innerException) : base(Resources.ReadTooLitte, innerException) {
		}

		protected IncompleteReadException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}