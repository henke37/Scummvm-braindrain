using System;

namespace DrainLib {
	[System.Serializable]
	public class InconsistentDataException : Exception {
		public InconsistentDataException() { }
		public InconsistentDataException(string message) : base(message) { }
		public InconsistentDataException(string message, Exception inner) : base(message, inner) { }
		protected InconsistentDataException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
