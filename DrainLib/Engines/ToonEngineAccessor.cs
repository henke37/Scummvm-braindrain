using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public class ToonEngineAccessor : BaseEngineAccessor {
		#region Symbol Data
		//Engine
		private uint gameStateOffset;
		private uint moviePlayerOffset;

		//State
		private uint gameGlobalDataOffset;
		private uint gameFlagOffset;
		private uint currentSceneOffset;
		private uint inventoryOffset;
		private uint confiscatedInventoryOffset;
		private uint numInventoryItemsOffset;
		private uint numConfiscatedInventoryItemsOffset;
		private const uint GameGlobalDataSize = 256;
		private const uint GameFlagDataSize = 256;

		//Movie
		private uint moviePlayingOffset;
		private uint movieDecoderOffset;
		private uint smkDecFileStreamOffset;
		#endregion

		internal ToonEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "toon";

		internal override void LoadSymbols() {
			var engineClSymb = Connector.resolver.FindClass("Toon::ToonEngine");
			gameStateOffset = Connector.resolver.FieldOffset(engineClSymb, "_gameState");
			moviePlayerOffset = Connector.resolver.FieldOffset(engineClSymb, "_moviePlayer");

			var stateClSymb = Connector.resolver.FindClass("Toon::State");
			gameGlobalDataOffset = Connector.resolver.FieldOffset(stateClSymb, "_gameGlobalData");
			gameFlagOffset = Connector.resolver.FieldOffset(stateClSymb, "_gameFlag");
			currentSceneOffset = Connector.resolver.FieldOffset(stateClSymb, "_currentScene");
			inventoryOffset = Connector.resolver.FieldOffset(stateClSymb, "_inventory");
			confiscatedInventoryOffset = Connector.resolver.FieldOffset(stateClSymb, "_confiscatedInventory");
			numInventoryItemsOffset = Connector.resolver.FieldOffset(stateClSymb, "_numInventoryItems");
			numConfiscatedInventoryItemsOffset = Connector.resolver.FieldOffset(stateClSymb, "_numConfiscatedInventoryItems");

			var movieClSymb = Connector.resolver.FindClass("Toon::Movie");
			moviePlayingOffset = Connector.resolver.FieldOffset(movieClSymb, "_playing");
			movieDecoderOffset = Connector.resolver.FieldOffset(movieClSymb, "_decoder");

			var smkDecClSymb = Connector.resolver.FindClass("Video::SmackerDecoder");
			smkDecFileStreamOffset = Connector.resolver.FieldOffset(smkDecClSymb, "_fileStream");
		}

		public ToonState GetState() {
			var statePtrVal = Connector.memoryReader.ReadUInt32(EngineAddr + gameStateOffset);

			var state = new ToonState();
			state.GlobalData = Connector.memoryReader.ReadInt16Array(statePtrVal + gameGlobalDataOffset, GameGlobalDataSize);
			state.flagData = Connector.memoryReader.ReadBytes(statePtrVal + gameFlagOffset, GameFlagDataSize);
			state.CurrentScene = Connector.memoryReader.ReadInt16(statePtrVal + currentSceneOffset);
			var numInventoryItems = Connector.memoryReader.ReadInt32(statePtrVal + numInventoryItemsOffset);
			state.Inventory = Connector.memoryReader.ReadInt16Array(statePtrVal + inventoryOffset, (uint)numInventoryItems);
			var numConfiscatedItems = Connector.memoryReader.ReadInt32(statePtrVal + numConfiscatedInventoryItemsOffset);
			state.ConfiscatedInventory = Connector.memoryReader.ReadInt16Array(statePtrVal + confiscatedInventoryOffset, (uint)numConfiscatedItems);
			return state;
		}

		public override VideoState GetVideoState() {
			var moviePlayerPtrVal = Connector.memoryReader.ReadUInt32(EngineAddr + moviePlayerOffset);
			bool playing = Connector.memoryReader.ReadByte(moviePlayerPtrVal + moviePlayingOffset)!=0;
			if(!playing) return null;
			var decoderPtrVal = Connector.memoryReader.ReadUInt32(moviePlayerPtrVal + movieDecoderOffset);
			var fileStreamPtrVal = Connector.memoryReader.ReadUInt32(decoderPtrVal + smkDecFileStreamOffset);

			var state = new VideoState();
			state.FileName = ReadFileName(fileStreamPtrVal);

			return state;
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
