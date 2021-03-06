﻿using System;
using System.Collections.Generic;
using Henke37.DebugHelp;
using Henke37.DebugHelp.PdbAccess;
using DIA;

namespace DrainLib.Engines {
	public class ScummEngineAccessor : BaseEngineAccessor {
		#region Symbol data
		//GameSettings
		private int gameIdOffset;
		private int variantOffset;
		private int versionOffset;
		private int heVersionOffset;

		//ScummEngine
		private int gameOffset;
		private int bootParamOffset;
		private int currentRoomOffset;
		private int numRoomsOffset;
		private int roomVarsOffset;
		private int scummVarsOffset;
		private int bitVarsOffset;
		private int inventoryOffset;
		private int classDataOffset;
		private int objectOwnerTableOffset;
		private int objectStateTableOffset;
		private int numVarsOffset;
		private int numRoomVarsOffset;
		private int numBitVarsOffset;
		private int numInventoryOffset;
		private int numGlobalObjectsOffset;
		private int resOffset;
		private int varWatchOffset;
		private int smushActiveOffset;
		private int smushPlayerOffset;

		//SmushPlayer
		private int smushPlayerNBFramesOffset;
		private int smushPlayerFrameOffset;
		private int smushPlayerSpeedOffset;
		private int smushPlayerSeekFileOffset;

		//ResourceManager
		private int resTypesOffset;
		private int resTypeDataSize;
		private int resArrStorageOffset;
		private int resouceSize;
		private int resourceAddressOffset;

		//ArrayHeader
		private int arrHeadDim1Offset;
		private int arrHeadDim2Offset;
		private int arrHeadTypeOffset;
		private int arrHeadDataOffset;

		#endregion

	#region Semistatic data
		public readonly GameSettings gameSettings;

		private uint roomVarCount;
		private IntPtr roomVarsPtr;

		private uint varCount;
		private IntPtr varsPtr;

		private uint bitVarByteCount;
		private IntPtr bitVarsPtr;

		private uint inventoryCount;
		private IntPtr inventoryPtr;

		private uint numGlobalObjects;

		private IntPtr classDataPtr;
		private IntPtr objectOwnerTablePtr;
		private IntPtr objectStateTablePtr;

		private IntPtr resManPtrVal;
	#endregion


		internal ScummEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			gameSettings = GetGameSettings();

			if(gameSettings.Version >= 7) {
				LoadSmushSymbols();
			}

			VarMap = BuildVarMap();

			LoadSemiStaticData();
		}

		private void LoadSemiStaticData() {
			if(gameSettings.HeVersion > 0) {
				roomVarCount = MemoryReader.ReadUInt32(EngineAddr + numRoomVarsOffset);
				roomVarsPtr = MemoryReader.ReadIntPtr(EngineAddr + roomVarsOffset);
				if(roomVarsPtr == IntPtr.Zero) throw new InconsistentDataException();
			}

			varCount = MemoryReader.ReadUInt32(EngineAddr + numVarsOffset);
			varsPtr = MemoryReader.ReadIntPtr(EngineAddr + scummVarsOffset);
			if(varsPtr == IntPtr.Zero) throw new InconsistentDataException();

			bitVarByteCount = MemoryReader.ReadUInt32(EngineAddr + numBitVarsOffset) / 8;
			bitVarsPtr = MemoryReader.ReadIntPtr(EngineAddr + scummVarsOffset);
			if(bitVarsPtr == IntPtr.Zero) throw new InconsistentDataException();

			inventoryCount = MemoryReader.ReadUInt32(EngineAddr + numInventoryOffset);
			inventoryPtr = MemoryReader.ReadIntPtr(EngineAddr + inventoryOffset);
			if(inventoryPtr == IntPtr.Zero) throw new InconsistentDataException();

			numGlobalObjects = MemoryReader.ReadUInt32(EngineAddr + numGlobalObjectsOffset);

			classDataPtr = MemoryReader.ReadIntPtr(EngineAddr + classDataOffset);
			if(classDataPtr == IntPtr.Zero) throw new InconsistentDataException();
			objectOwnerTablePtr = MemoryReader.ReadIntPtr(EngineAddr + objectOwnerTableOffset);
			if(objectOwnerTablePtr == IntPtr.Zero) throw new InconsistentDataException();
			objectStateTablePtr = MemoryReader.ReadIntPtr(EngineAddr + objectStateTableOffset);
			if(objectStateTablePtr == IntPtr.Zero) throw new InconsistentDataException();

			resManPtrVal = MemoryReader.ReadIntPtr(EngineAddr + resOffset);
			if(resManPtrVal == IntPtr.Zero) throw new InconsistentDataException();
		}

		internal override void LoadSymbols() {
			var engineClSymb = Resolver.FindClass("Scumm::ScummEngine");
			gameOffset = Resolver.FieldOffset(engineClSymb, "_game");
			bootParamOffset = Resolver.FieldOffset(engineClSymb, "_bootParam");
			currentRoomOffset = Resolver.FieldOffset(engineClSymb, "_currentRoom");
			numRoomsOffset = Resolver.FieldOffset(engineClSymb, "_numRooms");
			roomVarsOffset = Resolver.FieldOffset(engineClSymb, "_roomVars");
			scummVarsOffset = Resolver.FieldOffset(engineClSymb, "_scummVars");
			bitVarsOffset = Resolver.FieldOffset(engineClSymb, "_bitVars");
			inventoryOffset = Resolver.FieldOffset(engineClSymb, "_inventory");
			classDataOffset = Resolver.FieldOffset(engineClSymb, "_classData");
			objectOwnerTableOffset = Resolver.FieldOffset(engineClSymb, "_objectOwnerTable");
			objectStateTableOffset = Resolver.FieldOffset(engineClSymb, "_objectStateTable");
			numVarsOffset = Resolver.FieldOffset(engineClSymb, "_numVariables");
			numRoomVarsOffset = Resolver.FieldOffset(engineClSymb, "_numRoomVariables");
			numBitVarsOffset = Resolver.FieldOffset(engineClSymb, "_numBitVariables");
			numInventoryOffset = Resolver.FieldOffset(engineClSymb, "_numInventory");
			numGlobalObjectsOffset = Resolver.FieldOffset(engineClSymb, "_numGlobalObjects");
			resOffset = Resolver.FieldOffset(engineClSymb, "_res");
			varWatchOffset = Resolver.FieldOffset(engineClSymb, "_varwatch");

			var gameSettingsSymb = Resolver.FindClass("Scumm::GameSettings");
			gameIdOffset = Resolver.FieldOffset(gameSettingsSymb, "gameid");
			variantOffset = Resolver.FieldOffset(gameSettingsSymb, "variant");
			versionOffset = Resolver.FieldOffset(gameSettingsSymb, "version");
			heVersionOffset = Resolver.FieldOffset(gameSettingsSymb, "heversion");

			var resManClSymb = Resolver.FindClass("Scumm::ResourceManager");
			resTypesOffset = Resolver.FieldOffset(resManClSymb, "_types");
			var resTypeClSymb = Resolver.FindNestedClass(resManClSymb, "ResTypeData");
			resTypeDataSize = (int)resTypeClSymb.length;
			var resTypeArrClSymb = Resolver.GetBaseClass(resTypeClSymb);
			resArrStorageOffset = Resolver.FieldOffset(resTypeArrClSymb, "_storage");
			var resouceClSymb = Resolver.FindNestedClass(resManClSymb, "Resource");
			resouceSize = (int)resouceClSymb.length;
			resourceAddressOffset = Resolver.FieldOffset(resouceClSymb, "_address");

			var arrayHeaderClSymb = Resolver.FindClass("Scumm::ScummEngine_v6::ArrayHeader");
			arrHeadDim1Offset = Resolver.FieldOffset(arrayHeaderClSymb, "dim1");
			arrHeadDim2Offset = Resolver.FieldOffset(arrayHeaderClSymb, "dim2");
			arrHeadTypeOffset = Resolver.FieldOffset(arrayHeaderClSymb, "type");
			arrHeadDataOffset = Resolver.FieldOffset(arrayHeaderClSymb, "data");
		}

		private void LoadSmushSymbols() {
			var engine7ClSymb = Resolver.FindClass("Scumm::ScummEngine_v7");
			smushActiveOffset = Resolver.FieldOffset(engine7ClSymb, "_smushActive");
			smushPlayerOffset = Resolver.FieldOffset(engine7ClSymb, "_splayer");

			var smushPlayerClSymb = Resolver.FindClass("Scumm::SmushPlayer");
			smushPlayerNBFramesOffset = Resolver.FieldOffset(smushPlayerClSymb, "_nbframes");
			smushPlayerFrameOffset = Resolver.FieldOffset(smushPlayerClSymb, "_frame");
			smushPlayerSpeedOffset = Resolver.FieldOffset(smushPlayerClSymb, "_speed");
			smushPlayerSeekFileOffset = Resolver.FieldOffset(smushPlayerClSymb, "_seekFile");
		}

		public override string GameId => gameSettings.GameId;
		public override bool IsDemo => (gameSettings.GameFeatures & GameFeatures.Demo)!=0;

		public Dictionary<string, byte> VarMap { get; }

		private GameSettings GetGameSettings() {
			IntPtr addr = EngineAddr + gameOffset;

			var settings = new GameSettings();
			var gameIdPtrVal = MemoryReader.ReadIntPtr(addr + gameIdOffset);
			settings.GameId = gameIdPtrVal == IntPtr.Zero ? "" : MemoryReader.ReadNullTermString(gameIdPtrVal);
			var variantPtrVal = MemoryReader.ReadIntPtr(addr + variantOffset);
			settings.Variant = variantPtrVal == IntPtr.Zero ? "" : MemoryReader.ReadNullTermString(variantPtrVal);
			settings.Version = MemoryReader.ReadByte(addr + versionOffset);
			settings.HeVersion = MemoryReader.ReadByte(addr + heVersionOffset);
			return settings;
		}

		public override VideoState? GetVideoState() {
			if(gameSettings.Version < 7) return null;

			var active = MemoryReader.ReadByte(EngineAddr + smushActiveOffset) != 0;
			if(!active) return null;

			var addr = MemoryReader.ReadIntPtr(EngineAddr + smushPlayerOffset);
			if(addr == IntPtr.Zero) throw new InconsistentDataException();

			var state = new VideoState();
			state.CurrentFrame = MemoryReader.ReadUInt32(addr + smushPlayerFrameOffset);
			state.FrameCount = MemoryReader.ReadUInt32(addr + smushPlayerNBFramesOffset);
			state.FrameRate = new Rational(MemoryReader.ReadInt32(addr + smushPlayerSpeedOffset));
			state.FileName = ReadComString(addr + smushPlayerSeekFileOffset);
			return state;
		}

		public ScummState GetScummState() {
			var state = new ScummState();
			state.CurrentRoom = MemoryReader.ReadByte(EngineAddr + currentRoomOffset);

			if(gameSettings.HeVersion > 0) {
				state.RoomVars = MemoryReader.ReadInt32Array(roomVarsPtr, roomVarCount);
			}
			
			state.ScummVars = MemoryReader.ReadInt32Array(varsPtr, varCount);
			state.bitVarData = MemoryReader.ReadBytes(bitVarsPtr, bitVarByteCount);
			
			state.Inventory = MemoryReader.ReadInt16Array(inventoryPtr, inventoryCount);
			
			state.GlobalObjectClasses = MemoryReader.ReadUInt32Array(classDataPtr, numGlobalObjects);
			state.GlobalObjectOwners = MemoryReader.ReadBytes(objectOwnerTablePtr, numGlobalObjects);
			state.GlobalObjectStates = MemoryReader.ReadBytes(objectStateTablePtr, numGlobalObjects);

			return state;
		}

		private Dictionary<string, byte> BuildVarMap() {
			var engineClSymb = Resolver.FindClass("Scumm::ScummEngine");
			var varSymbols=engineClSymb.findChildren(
				SymTagEnum.Data,
				"VAR_*",
				NameSearchOptions.RegularExpression
			);

			var map = new Dictionary<string, byte>(varSymbols.count);

			foreach(IDiaSymbol varSymb in varSymbols) {
				string varName = varSymb.name;
				byte varId = MemoryReader.ReadByte(EngineAddr + varSymb.offset);
				if(varId == 0xFF) continue;
				map.Add(varName, varId);
			}

			return map;
		}

		public void ReadArray(int arrayId) {
			if(gameSettings.Version < 6) throw new InvalidOperationException("Arrays aren't used in this game");
			var arrayHeaderAddr = GetResourceAddr(ResourceType.String, arrayId);

			short dim1 = MemoryReader.ReadInt16(arrayHeaderAddr + arrHeadDim1Offset);
			short dim2 = MemoryReader.ReadInt16(arrayHeaderAddr + arrHeadDim2Offset);
			ArrayType type = (ArrayType)MemoryReader.ReadInt16(arrayHeaderAddr + arrHeadTypeOffset);

			uint numElements = (uint)(dim1 * dim2);

			var dataAddr = arrayHeaderAddr + arrHeadDataOffset;
			if(type != ArrayType.IntArray) {
				MemoryReader.ReadBytes(dataAddr, numElements);
			} else if(gameSettings.Version == 8) {
				MemoryReader.ReadUInt32Array(dataAddr, numElements);
			} else {
				MemoryReader.ReadUInt16Array(dataAddr, numElements);
			}
		}

		private IntPtr GetResourceAddr(ResourceType type, int index) {
			var typesBase = resManPtrVal + resTypesOffset;
			IntPtr typeLocation = typesBase + (int)type * resTypeDataSize;
			var resStorageBase = MemoryReader.ReadIntPtr(typeLocation + resArrStorageOffset);
			if(resStorageBase == IntPtr.Zero) throw new InconsistentDataException();
			IntPtr resLocation = resStorageBase + resouceSize * index;

			return MemoryReader.ReadIntPtr(resLocation + resourceAddressOffset);
		}

		public short VarWatch {
			get {
				return MemoryReader.ReadInt16(EngineAddr + varWatchOffset);
			}
			set {
				MemoryReader.WriteInt16(EngineAddr + varWatchOffset, value);
			}
		}

		public int BootParam => MemoryReader.ReadInt32(EngineAddr + bootParamOffset);

		public byte RoomCount => MemoryReader.ReadByte(EngineAddr + numRoomsOffset);

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

		[Serializable]
		public class GameSettings {
			public string GameId;
			public string Variant;
			public int Version;
			public int HeVersion;
			public GameFeatures GameFeatures;
		}

		[Flags]
		public enum GameFeatures {
			Demo = 1 << 0,
			NewCostumes = 1 << 2,
			UseKey = 1 << 4,
			SmallHeader = 1 << 5,
			OldBundle = 1 << 6,
			Color16 = 1 << 7,
			Old256 = 1 << 8,
			AudioTracks = 1 << 9,
			FewLocals = 1 << 11,
			HELocalized = 1 << 13,
			HE985 = 1 << 14,
			Color16Bit = 1 << 15,
			MacContainer = 1 << 16
		}
	}

	[Serializable]
	public class ScummState {
		public byte CurrentRoom;

		public int[] RoomVars;
		public int[] ScummVars;
		public short[] Inventory;
		public uint[] GlobalObjectClasses;
		public byte[] GlobalObjectOwners;
		public byte[] GlobalObjectStates;

		public bool GetBitVar(uint varNum) {
			return (bitVarData[varNum >> 3] & (1 << (int)(varNum & 7))) != 0;
		}

		public bool HasItem(short item) {
			return Array.IndexOf(Inventory, item) != -1;
		}

		public byte[] bitVarData;
	}


}
