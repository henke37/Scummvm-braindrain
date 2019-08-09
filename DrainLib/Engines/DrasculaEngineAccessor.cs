using DrainLib.Engines;
using System;

namespace DrainLib {
	public class DrasculaEngineAccessor : ADBaseEngineAccessor {
		#region Symbol data
		private int roomNumberOffset;
		private int flagsOffset;
		private const uint NumFlags = 50;
		private int currentChapterOffset;
		private int inventoryObjectsOffset;
		private const uint InventorySize = 43;
		#endregion

		public DrasculaEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineClSymb = Connector.resolver.FindClass("Drascula::DrasculaEngine");
			var descOffset = Connector.resolver.FieldOffset(engineClSymb, "_gameDescription");
			roomNumberOffset = Connector.resolver.FieldOffset(engineClSymb, "_roomNumber");
			flagsOffset = Connector.resolver.FieldOffset(engineClSymb, "flags");
			currentChapterOffset = Connector.resolver.FieldOffset(engineClSymb, "currentChapter");
			inventoryObjectsOffset = Connector.resolver.FieldOffset(engineClSymb, "inventoryObjects");

			LoadADSymbols(descOffset, true);
		}

		public DrasculaState GetState() {
			var state = new DrasculaState();
			state.RoomNumber = Connector.memoryReader.ReadInt32(EngineAddr + roomNumberOffset);
			state.Flags = Connector.memoryReader.ReadInt32Array(EngineAddr + flagsOffset, NumFlags);
			state.CurrentChapter = Connector.memoryReader.ReadInt32(EngineAddr + currentChapterOffset);
			state.Inventory = Connector.memoryReader.ReadInt32Array(EngineAddr + inventoryObjectsOffset, InventorySize);
			return state;
		}
	}

	public class DrasculaState {
		public int RoomNumber;
		public int[] Flags;
		public int CurrentChapter;
		public int[] Inventory;
	}
}