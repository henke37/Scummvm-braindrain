﻿using System;
using System.Collections.Generic;
using System.IO;

namespace DrainLib.Engines {
	public class QueenEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		private int logicOffset;
		private int currentRoomOffset;
		private int gameStateOffset;
		private const uint gameStateCount = 211;
		private int numItemsOffset;
		private int itemDataOffset;
		#endregion

		internal QueenEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "queen";

		internal override void LoadSymbols() {
			var engineSymb = Resolver.FindClass("Queen::QueenEngine");
			logicOffset = Resolver.FieldOffset(engineSymb, "_logic");

			var logicClSymb = Resolver.FindClass("Queen::Logic");
			currentRoomOffset = Resolver.FieldOffset(logicClSymb, "_currentRoom");
			gameStateOffset = Resolver.FieldOffset(logicClSymb, "_gameState");
			numItemsOffset = Resolver.FieldOffset(logicClSymb, "_numItems");
			itemDataOffset = Resolver.FieldOffset(logicClSymb, "_itemData");
		}

		public QueenState GetState() {
			var state = new QueenState();

			var logicPtrVal = MemoryReader.ReadIntPtr(EngineAddr + logicOffset);

			string logicName=RttiReader.GetMangledClassNameFromObjPtr(logicPtrVal);
			switch(logicName) {
				case ".?AVLogicGame@Queen@@":
					state.LogicMode = LogicMode.Game;
					break;
				case ".?AVLogicDemo@Queen@@":
					state.LogicMode = LogicMode.Demo;
					break;
				case ".?AVLogicInterview@Queen@@":
					state.LogicMode = LogicMode.Interview;
					break;
				default:
					throw new InvalidDataException("Unrecognized logic type");
			}

			state.CurrentRoom = MemoryReader.ReadUInt16(logicPtrVal + currentRoomOffset);
			state.GameState = MemoryReader.ReadInt16Array(logicPtrVal + gameStateOffset, gameStateCount);

			state.Inventory = ReadInventory(logicPtrVal);

			return state;
		}

		private QueenState.ItemData[] ReadInventory(IntPtr logicPtrVal) {
			var numItems = MemoryReader.ReadUInt16(logicPtrVal + numItemsOffset);
			var itemDataPtrVal = MemoryReader.ReadIntPtr(logicPtrVal + itemDataOffset);

			return MemoryReader.ReadStructArr<QueenState.ItemData>(itemDataPtrVal, numItems);
		}
	}

	public class QueenState {
		public uint CurrentRoom;
		public short[] GameState;
		public LogicMode LogicMode;
		public ItemData[] Inventory;


		public struct ItemData {
			public short NameId;
			public ushort DescId;
			public ItemState State;
			public ushort Frame;
			public short SfxDesc;
		}

		[Flags]
		public enum ItemState : ushort {
			GrabNone = 0,
			GrabDown = 1,
			GrabUp = 2,
			GrabMid = 3,

			DirBack = 0,
			DirRight = 1 << 2,
			DirLeft = 2 << 2,
			DirFront = 3 << 2,

			VerbNone = 0,
			VerbOpen = 1 << 4,
			VerbClose = 3 << 4,
			VerbLookAt = 6 << 4,
			VerbMove = 7 << 4,
			VerbGive = 8 << 4,
			VerbTalkTo = 9 << 4,
			VerbUse = 12 << 4,
			VerbPickUp = 14 << 4,

			On = 1 << 8,
			Talk = 1 << 9,
			Use = 1 << 10
		}
	}

	public enum LogicMode {
		Game,
		Demo,
		Interview
	}
}
