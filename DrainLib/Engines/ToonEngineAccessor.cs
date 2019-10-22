using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class ToonEngineAccessor : BaseEngineAccessor {
		#region Symbol Data
		//Engine
		private int gameStateOffset;
		private int moviePlayerOffset;

		//State
		private int gameGlobalDataOffset;
		private int gameFlagOffset;
		private int currentSceneOffset;
		private int inventoryOffset;
		private int confiscatedInventoryOffset;
		private int numInventoryItemsOffset;
		private int numConfiscatedInventoryItemsOffset;
		private const uint GameGlobalDataSize = 256;
		private const uint GameFlagDataSize = 256;

		//Movie
		private int moviePlayingOffset;
		private int movieDecoderOffset;
		#endregion

		private VideoAccessor videoAccessor;

		internal ToonEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			videoAccessor = new VideoAccessor(connector);
		}

		public override string GameId => "toon";

		internal override void LoadSymbols() {
			var engineClSymb = Resolver.FindClass("Toon::ToonEngine");
			gameStateOffset = Resolver.FieldOffset(engineClSymb, "_gameState");
			moviePlayerOffset = Resolver.FieldOffset(engineClSymb, "_moviePlayer");

			var stateClSymb = Resolver.FindClass("Toon::State");
			gameGlobalDataOffset = Resolver.FieldOffset(stateClSymb, "_gameGlobalData");
			gameFlagOffset = Resolver.FieldOffset(stateClSymb, "_gameFlag");
			currentSceneOffset = Resolver.FieldOffset(stateClSymb, "_currentScene");
			inventoryOffset = Resolver.FieldOffset(stateClSymb, "_inventory");
			confiscatedInventoryOffset = Resolver.FieldOffset(stateClSymb, "_confiscatedInventory");
			numInventoryItemsOffset = Resolver.FieldOffset(stateClSymb, "_numInventoryItems");
			numConfiscatedInventoryItemsOffset = Resolver.FieldOffset(stateClSymb, "_numConfiscatedInventoryItems");

			var movieClSymb = Resolver.FindClass("Toon::Movie");
			moviePlayingOffset = Resolver.FieldOffset(movieClSymb, "_playing");
			movieDecoderOffset = Resolver.FieldOffset(movieClSymb, "_decoder");
		}

		public ToonState GetState() {
			var statePtrVal = MemoryReader.ReadIntPtr(EngineAddr + gameStateOffset);

			var state = new ToonState();
			state.GlobalData = MemoryReader.ReadInt16Array(statePtrVal + gameGlobalDataOffset, GameGlobalDataSize);
			state.flagData = MemoryReader.ReadBytes(statePtrVal + gameFlagOffset, GameFlagDataSize);
			state.CurrentScene = MemoryReader.ReadInt16(statePtrVal + currentSceneOffset);
			var numInventoryItems = MemoryReader.ReadInt32(statePtrVal + numInventoryItemsOffset);
			state.Inventory = MemoryReader.ReadInt16Array(statePtrVal + inventoryOffset, (uint)numInventoryItems);
			var numConfiscatedItems = MemoryReader.ReadInt32(statePtrVal + numConfiscatedInventoryItemsOffset);
			state.ConfiscatedInventory = MemoryReader.ReadInt16Array(statePtrVal + confiscatedInventoryOffset, (uint)numConfiscatedItems);
			return state;
		}

		public override VideoState? GetVideoState() {
			var moviePlayerPtrVal = MemoryReader.ReadIntPtr(EngineAddr + moviePlayerOffset);
			bool playing = MemoryReader.ReadByte(moviePlayerPtrVal + moviePlayingOffset)!=0;
			if(!playing) return null;
			var decoderPtrVal = MemoryReader.ReadIntPtr(moviePlayerPtrVal + movieDecoderOffset);

			return videoAccessor.ReadDecoder(decoderPtrVal);
		}

		public class ToonState {
			internal byte[] flagData;
			public short CurrentScene;
			public short[] Inventory;
			public short[] ConfiscatedInventory;
			public short[] GlobalData;

			public ToonState() {
			}
		}
	}
}
