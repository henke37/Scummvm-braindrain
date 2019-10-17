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
			var engineClSymb = Resolver.FindClass("Drascula::DrasculaEngine");
			var descOffset = Resolver.FieldOffset(engineClSymb, "_gameDescription");
			roomNumberOffset = Resolver.FieldOffset(engineClSymb, "_roomNumber");
			flagsOffset = Resolver.FieldOffset(engineClSymb, "flags");
			currentChapterOffset = Resolver.FieldOffset(engineClSymb, "currentChapter");
			inventoryObjectsOffset = Resolver.FieldOffset(engineClSymb, "inventoryObjects");

			LoadADSymbols(descOffset, true);
		}

		public DrasculaState GetState() {
			var state = new DrasculaState();
			state.RoomNumber = MemoryReader.ReadInt32(EngineAddr + roomNumberOffset);
			state.Flags = MemoryReader.ReadInt32Array(EngineAddr + flagsOffset, NumFlags);
			state.CurrentChapter = MemoryReader.ReadInt32(EngineAddr + currentChapterOffset);
			state.Inventory = MemoryReader.ReadInt32Array(EngineAddr + inventoryObjectsOffset, InventorySize);
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