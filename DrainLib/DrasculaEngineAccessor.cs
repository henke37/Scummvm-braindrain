using DrainLib.Engines;

namespace DrainLib {
	public class DrasculaEngineAccessor : ADBaseEngineAccessor {
		private uint roomNumberOffset;

		public DrasculaEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineClSymb = Connector.resolver.FindClass("Drascula::DrasculaEngine");
			var descOffset = Connector.resolver.FieldOffset(engineClSymb, "_gameDescription");
			roomNumberOffset = Connector.resolver.FieldOffset(engineClSymb, "_roomNumber");

			LoadADSymbols(descOffset, true);
		}

		public DrasculaState GetState() {
			var state = new DrasculaState();
			state.RoomNumber = Connector.memoryReader.ReadInt32(EngineAddr + roomNumberOffset);
			return state;
		}
	}

	public class DrasculaState {
		public int RoomNumber;
	}
}