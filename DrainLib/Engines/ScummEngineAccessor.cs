using System;
using System.Collections.Generic;
using DebugHelp;
using Dia2Lib;

namespace DrainLib.Engines {
	public class ScummEngineAccessor : BaseEngineAccessor {
		#region Symbol data
		//GameSettings
		private uint gameIdOffset;
		private uint variantOffset;
		private uint versionOffset;
		private uint heVersionOffset;

		//ScummEngine
		private uint gameOffset;
		private uint bootParamOffset;
		private uint currentRoomOffset;
		private uint numRoomsOffset;
		private uint roomVarsOffset;
		private uint scummVarsOffset;
		private uint bitVarsOffset;
		private uint inventoryOffset;
		private uint classDataOffset;
		private uint objectOwnerTableOffset;
		private uint objectStateTableOffset;
		private uint numVarsOffset;
		private uint numRoomVarsOffset;
		private uint numBitVarsOffset;
		private uint numInventoryOffset;
		private uint numGlobalObjectsOffset;
		private uint resOffset;
		private uint smushActiveOffset;
		private uint smushPlayerOffset;

		//SmushPlayer
		private uint smushPlayerNBFramesOffset;
		private uint smushPlayerFrameOffset;
		private uint smushPlayerSpeedOffset;
		private uint smushPlayerSeekFileOffset;

		//ResourceManager
		private uint resTypesOffset;
		private ulong resTypeDataSize;
		private uint resArrStorageOffset;
		private ulong resouceSize;
		private uint resourceAddressOffset;

		//ArrayHeader
		private uint arrHeadDim1Offset;
		private uint arrHeadDim2Offset;
		private uint arrHeadTypeOffset;
		private uint arrHeadDataOffset;

		#endregion

		public readonly GameSettings GameSettings;

		internal ScummEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
			GameSettings = GetGameSettings();

			if(GameSettings.Version >= 7) {
				LoadSmushSymbols();
			}

			VarMap = BuildVarMap();
		}

		internal override void LoadSymbols() {
			var engineClSymb = Connector.resolver.FindClass("Scumm::ScummEngine");
			gameOffset = Connector.resolver.FieldOffset(engineClSymb, "_game");
			bootParamOffset = Connector.resolver.FieldOffset(engineClSymb, "_bootParam");
			currentRoomOffset = Connector.resolver.FieldOffset(engineClSymb, "_currentRoom");
			numRoomsOffset = Connector.resolver.FieldOffset(engineClSymb, "_numRooms");
			roomVarsOffset = Connector.resolver.FieldOffset(engineClSymb, "_roomVars");
			scummVarsOffset = Connector.resolver.FieldOffset(engineClSymb, "_scummVars");
			bitVarsOffset = Connector.resolver.FieldOffset(engineClSymb, "_bitVars");
			inventoryOffset = Connector.resolver.FieldOffset(engineClSymb, "_inventory");
			classDataOffset = Connector.resolver.FieldOffset(engineClSymb, "_classData");
			objectOwnerTableOffset = Connector.resolver.FieldOffset(engineClSymb, "_objectOwnerTable");
			objectStateTableOffset = Connector.resolver.FieldOffset(engineClSymb, "_objectStateTable");
			numVarsOffset = Connector.resolver.FieldOffset(engineClSymb, "_numVariables");
			numRoomVarsOffset = Connector.resolver.FieldOffset(engineClSymb, "_numRoomVariables");
			numBitVarsOffset = Connector.resolver.FieldOffset(engineClSymb, "_numBitVariables");
			numInventoryOffset = Connector.resolver.FieldOffset(engineClSymb, "_numInventory");
			numGlobalObjectsOffset = Connector.resolver.FieldOffset(engineClSymb, "_numGlobalObjects");
			resOffset = Connector.resolver.FieldOffset(engineClSymb, "_res");

			var gameSettingsSymb = Connector.resolver.FindClass("Scumm::GameSettings");
			gameIdOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "gameid");
			variantOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "variant");
			versionOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "version");
			heVersionOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "heversion");

			var resManClSymb = Connector.resolver.FindClass("Scumm::ResourceManager");
			resTypesOffset = Connector.resolver.FieldOffset(resManClSymb, "_types");
			var resTypeClSymb = Connector.resolver.FindNestedClass(resManClSymb, "ResTypeData");
			resTypeDataSize = resTypeClSymb.length;
			var resTypeArrClSymb = Connector.resolver.GetBaseClass(resTypeClSymb);
			resArrStorageOffset = Connector.resolver.FieldOffset(resTypeArrClSymb, "_storage");
			var resouceClSymb = Connector.resolver.FindNestedClass(resManClSymb, "Resource");
			resouceSize = resouceClSymb.length;
			resourceAddressOffset = Connector.resolver.FieldOffset(resouceClSymb, "_address");

			var arrayHeaderClSymb = Connector.resolver.FindClass("Scumm::ScummEngine_v6::ArrayHeader");
			arrHeadDim1Offset = Connector.resolver.FieldOffset(arrayHeaderClSymb, "dim1");
			arrHeadDim2Offset = Connector.resolver.FieldOffset(arrayHeaderClSymb, "dim2");
			arrHeadTypeOffset = Connector.resolver.FieldOffset(arrayHeaderClSymb, "type");
			arrHeadDataOffset = Connector.resolver.FieldOffset(arrayHeaderClSymb, "data");
		}

		private void LoadSmushSymbols() {
			var engine7ClSymb = Connector.resolver.FindClass("Scumm::ScummEngine_v7");
			smushActiveOffset = Connector.resolver.FieldOffset(engine7ClSymb, "_smushActive");
			smushPlayerOffset = Connector.resolver.FieldOffset(engine7ClSymb, "_splayer");

			var smushPlayerClSymb = Connector.resolver.FindClass("Scumm::SmushPlayer");
			smushPlayerNBFramesOffset = Connector.resolver.FieldOffset(smushPlayerClSymb, "_nbframes");
			smushPlayerFrameOffset = Connector.resolver.FieldOffset(smushPlayerClSymb, "_frame");
			smushPlayerSpeedOffset = Connector.resolver.FieldOffset(smushPlayerClSymb, "_speed");
			smushPlayerSeekFileOffset = Connector.resolver.FieldOffset(smushPlayerClSymb, "_seekFile");
		}

		public override string GameId => GameSettings.GameId;

		public Dictionary<string, byte> VarMap { get; }

		private GameSettings GetGameSettings() {
			uint addr = EngineAddr + gameOffset;

			var settings = new GameSettings();
			var gameIdPtrVal = Connector.memoryReader.ReadUInt32(addr + gameIdOffset);
			settings.GameId = gameIdPtrVal == 0 ? "" : Connector.memoryReader.ReadNullTermString(gameIdPtrVal);
			var variantPtrVal = Connector.memoryReader.ReadUInt32(addr + variantOffset);
			settings.Variant = variantPtrVal == 0 ? "" : Connector.memoryReader.ReadNullTermString(variantPtrVal);
			settings.Version = Connector.memoryReader.ReadByte(addr + versionOffset);
			settings.HeVersion = Connector.memoryReader.ReadByte(addr + heVersionOffset);
			return settings;
		}

		public override VideoState GetVideoState() {
			if(GameSettings.Version < 7) return null;

			var active = Connector.memoryReader.ReadByte(EngineAddr + smushActiveOffset) != 0;
			if(!active) return null;

			var addr = Connector.memoryReader.ReadUInt32(EngineAddr + smushPlayerOffset);

			var state = new VideoState();
			state.CurrentFrame = Connector.memoryReader.ReadUInt32(addr + smushPlayerFrameOffset);
			state.FrameCount = Connector.memoryReader.ReadUInt32(addr + smushPlayerNBFramesOffset);
			state.FrameRate = Connector.memoryReader.ReadInt32(addr + smushPlayerSpeedOffset);
			state.FileName = ReadComString(addr + smushPlayerSeekFileOffset);
			return state;
		}

		public ScummState GetScummState() {
			var state = new ScummState();
			state.CurrentRoom = Connector.memoryReader.ReadByte(EngineAddr + currentRoomOffset);
			state.RoomCount = Connector.memoryReader.ReadByte(EngineAddr + numRoomsOffset);
			state.BootParam = Connector.memoryReader.ReadInt32(EngineAddr + bootParamOffset);

			if(GameSettings.HeVersion > 0) {
				var roomVarCount = Connector.memoryReader.ReadInt32(EngineAddr + numRoomVarsOffset);
				var roomVarsPtr = Connector.memoryReader.ReadUInt32(EngineAddr + roomVarsOffset);
				state.RoomVars = Connector.memoryReader.ReadInt32Array(roomVarsPtr, (uint)roomVarCount);
			}

			{
				var varCount = Connector.memoryReader.ReadInt32(EngineAddr + numVarsOffset);
				var varsPtr = Connector.memoryReader.ReadUInt32(EngineAddr + scummVarsOffset);
				state.ScummVars = Connector.memoryReader.ReadInt32Array(varsPtr, (uint)varCount);
			}

			{
				var bitVarByteCount = Connector.memoryReader.ReadInt32(EngineAddr + numBitVarsOffset) / 8;
				var bitVarsPtr = Connector.memoryReader.ReadUInt32(EngineAddr + scummVarsOffset);
				state.bitVarData = Connector.memoryReader.ReadBytes(bitVarsPtr, (uint)bitVarByteCount);
			}

			{
				var inventoryCount = Connector.memoryReader.ReadInt32(EngineAddr + numInventoryOffset);
				var inventoryPtr = Connector.memoryReader.ReadUInt32(EngineAddr + inventoryOffset);
				state.Inventory = Connector.memoryReader.ReadInt16Array(inventoryPtr, (uint)inventoryCount);
			}

			uint numGlobalObjects = (uint)Connector.memoryReader.ReadInt32(EngineAddr + numGlobalObjectsOffset);
			{
				var classDataPtr = Connector.memoryReader.ReadUInt32(EngineAddr + classDataOffset);
				state.GlobalObjectClasses = Connector.memoryReader.ReadUInt32Array(classDataPtr, numGlobalObjects);
			}
			{
				var objectOwnerTablePtr = Connector.memoryReader.ReadUInt32(EngineAddr + objectOwnerTableOffset);
				state.GlobalObjectOwners = Connector.memoryReader.ReadBytes(objectOwnerTablePtr, numGlobalObjects);
			}
			{
				var objectStateTablePtr = Connector.memoryReader.ReadUInt32(EngineAddr + objectStateTableOffset);
				state.GlobalObjectStates = Connector.memoryReader.ReadBytes(objectStateTablePtr, numGlobalObjects);
			}

			return state;
		}

		private Dictionary<string, byte> BuildVarMap() {
			var engineClSymb = Connector.resolver.FindClass("Scumm::ScummEngine");
			engineClSymb.findChildren(
				Dia2Lib.SymTagEnum.SymTagData,
				"VAR_*",
				(uint)NameSearchOptions.Glob,
				out var varSymbols
			);

			var map = new Dictionary<string, byte>(varSymbols.count);

			foreach(IDiaSymbol varSymb in varSymbols) {
				string varName = varSymb.name;
				byte varId = Connector.memoryReader.ReadByte(EngineAddr + (uint)varSymb.offset);
				if(varId == 0xFF) continue;
				map.Add(varName, varId);
			}

			return map;
		}

		public void ReadArray(uint arrayId) {
			if(GameSettings.Version < 6) throw new InvalidOperationException("Arrays aren't used in this game");
			var arrayHeaderAddr = GetResourceAddr(ResourceType.String, arrayId);

			short dim1 = Connector.memoryReader.ReadInt16(arrayHeaderAddr + arrHeadDim1Offset);
			short dim2 = Connector.memoryReader.ReadInt16(arrayHeaderAddr + arrHeadDim2Offset);
			ArrayType type = (ArrayType)Connector.memoryReader.ReadInt16(arrayHeaderAddr + arrHeadTypeOffset);

			uint numElements = (uint)(dim1 * dim2);

			uint dataAddr = arrayHeaderAddr + arrHeadDataOffset;
			if(type != ArrayType.IntArray) {
				Connector.memoryReader.ReadBytes(dataAddr, numElements);
			} else if(GameSettings.Version == 8) {
				Connector.memoryReader.ReadUInt32Array(dataAddr, numElements);
			} else {
				Connector.memoryReader.ReadUInt16Array(dataAddr, numElements);
			}
		}

		private uint GetResourceAddr(ResourceType type, uint index) {
			var resManPtrVal = Connector.memoryReader.ReadUInt32(EngineAddr + resOffset);
			var typesBase = resManPtrVal + resTypesOffset;
			uint typeLocation = (uint)(typesBase + (uint)type * resTypeDataSize);
			var resStorageBase = Connector.memoryReader.ReadUInt32(typeLocation + resArrStorageOffset);
			uint resLocation = (uint)(resStorageBase + resouceSize * index);

			return Connector.memoryReader.ReadUInt32(resLocation + resourceAddressOffset);
		}

		private enum ResourceType {
			Invalid = 0,
			First = 1,
			Room = 1,
			Script = 2,
			Costume = 3,
			Sound = 4,
			Inventory = 5,
			Charset = 6,
			String = 7,
			Verb = 8,
			ActorName = 9,
			Buffer = 10,
			ScaleTable = 11,
			Temp = 12,
			FlObject = 13,
			Matrix = 14,
			Box = 15,
			ObjectName = 16,
			RoomScripts = 17,
			RoomImage = 18,
			Image = 19,
			Talkie = 20,
			SpoolBuffer = 21,
			Last = 21
		}
		private enum ArrayType {
			BitArray=1,
			NibbleType=2,
			ByteArray=3,
			StringArray=4,
			IntArray=5,
			DWordArray=6
		}
	}

	[Serializable]
	public class ScummState {
		public byte CurrentRoom;
		public byte RoomCount;

		public int BootParam;

		public int[] RoomVars;
		public int[] ScummVars;
		public short[] Inventory;
		public uint[] GlobalObjectClasses;
		public byte[] GlobalObjectOwners;
		public byte[] GlobalObjectStates;

		public bool GetBitVar(uint varNum) {
			return (bitVarData[varNum / 8] >> ((int)(varNum % 8))) != 0;
		}

		public byte[] bitVarData;
	}

	[Serializable]
	public class GameSettings {
		public string GameId;
		public string Variant;
		public int Version;
		public int HeVersion;
	}
}
