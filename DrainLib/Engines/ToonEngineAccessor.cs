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
		//Decoder
		private int smkDecFileStreamOffset;
		private int videoDecNextVideoTrackOffset;
		//Video track
		private int smkTrackCurFrameOffset;
		private int smkTrackFrameCountOffset;
		private int smkTrackFrameRateOffset;
		#endregion

		internal ToonEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
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

			var videoDecClSymb = Connector.resolver.FindClass("Video::VideoDecoder");
			videoDecNextVideoTrackOffset = Connector.resolver.FieldOffset(videoDecClSymb, "_nextVideoTrack");

			var smkVideoTrackClSymb = Connector.resolver.FindNestedClass(smkDecClSymb, "SmackerVideoTrack");
			smkTrackCurFrameOffset = Connector.resolver.FieldOffset(smkVideoTrackClSymb, "_curFrame");
			smkTrackFrameCountOffset = Connector.resolver.FieldOffset(smkVideoTrackClSymb, "_frameCount");
			smkTrackFrameRateOffset = Connector.resolver.FieldOffset(smkVideoTrackClSymb, "_frameRate");
		}

		public ToonState GetState() {
			var statePtrVal = Connector.memoryReader.ReadIntPtr(EngineAddr + gameStateOffset);

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
			var moviePlayerPtrVal = Connector.memoryReader.ReadIntPtr(EngineAddr + moviePlayerOffset);
			bool playing = Connector.memoryReader.ReadByte(moviePlayerPtrVal + moviePlayingOffset)!=0;
			if(!playing) return null;
			var decoderPtrVal = Connector.memoryReader.ReadIntPtr(moviePlayerPtrVal + movieDecoderOffset);
			var fileStreamPtrVal = Connector.memoryReader.ReadIntPtr(decoderPtrVal + smkDecFileStreamOffset);
			var videoTrackPtrVal = Connector.memoryReader.ReadIntPtr(decoderPtrVal + videoDecNextVideoTrackOffset);

			var state = new VideoState();
			state.FileName = ReadFileName(fileStreamPtrVal);
			state.CurrentFrame = Connector.memoryReader.ReadUInt32(videoTrackPtrVal + smkTrackCurFrameOffset);
			state.FrameCount = (uint)Connector.memoryReader.ReadInt32(videoTrackPtrVal + smkTrackFrameCountOffset);
			state.FrameRate=(float)ReadRational(videoTrackPtrVal + smkTrackFrameRateOffset);
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
