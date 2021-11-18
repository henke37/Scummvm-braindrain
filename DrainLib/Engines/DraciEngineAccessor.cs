using System;

namespace DrainLib.Engines {
	public class DraciEngineAccessor : BaseEngineAccessor {
		#region Symbol data
		//engine class
		private int gameOffset;

		//game class
		private int infoOffset;
		private int variablesOffset;
		private int itemsOffset;
		private int currentItemOffset;
		private int currentRoomOffset;

		//room class
		private int roomNumOffset;
		#endregion

		#region Semistatic data
		private IntPtr gameAddr;

		private class GameInfo {
			internal uint NumObjects;
			internal uint Numitems;
			internal byte NumVariables;
			internal byte NumPersons;
		}
		private GameInfo gameInfo;
		#endregion
		public DraciEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
			gameInfo=LoadGameInfo();
		}

		private void LoadSemiStaticData() {
			gameAddr = MemoryReader.ReadIntPtr(EngineAddr + gameOffset);
		}

		public override string GameId => "draci";

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Draci::DraciEngine");
			gameOffset = Resolver.FieldOffset(engineCl, "_game");

			var gameCl = Resolver.FindClass("Draci::Game");
			infoOffset = Resolver.FieldOffset(gameCl, "_info");
			variablesOffset = Resolver.FieldOffset(gameCl, "_variables");
			itemsOffset = Resolver.FieldOffset(gameCl, "_items");
			currentItemOffset = Resolver.FieldOffset(gameCl, "_currentItem");
			currentRoomOffset = Resolver.FieldOffset(gameCl, "_currentRoom");

			var roomCl = Resolver.FindClass("Draci::Room");
			roomNumOffset = Resolver.FieldOffset(roomCl, "_roomNum");
		}

		private GameInfo LoadGameInfo() {
			var giCl = Resolver.FindClass("Draci::GameInfo");

			var info = new GameInfo();

			var infoAddr = gameAddr + infoOffset;

			info.NumObjects = MemoryReader.ReadUInt32(infoAddr + Resolver.FieldOffset(giCl, "_numObjects"));
			info.Numitems = MemoryReader.ReadUInt32(infoAddr + Resolver.FieldOffset(giCl, "_numItems"));
			info.NumVariables = MemoryReader.ReadByte(infoAddr + Resolver.FieldOffset(giCl, "_numVariables"));
			info.NumPersons = MemoryReader.ReadByte(infoAddr + Resolver.FieldOffset(giCl, "_numPersons"));

			return info;
		}

		public GameState GetGameState() {
			var state = new GameState();
			state.CurrentRoom = MemoryReader.ReadInt32(gameAddr + currentRoomOffset + roomNumOffset);
			state.Variables = MemoryReader.ReadInt32Array(gameAddr + variablesOffset, gameInfo.NumVariables);
			return state;
		}

		public class GameState {
			public int CurrentRoom;
			public int[] Variables;
		}
	}
}
