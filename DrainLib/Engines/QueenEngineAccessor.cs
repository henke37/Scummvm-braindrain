using System;
using System.Collections.Generic;
using System.IO;

namespace DrainLib.Engines {
	public class QueenEngineAccessor : BaseEngineAccessor {
		private uint logicOffset;
		private uint currentRoomOffset;
		private uint gameStateOffset;
		private const uint gameStateCount = 211;

		internal QueenEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "queen";

		internal override void LoadSymbols() {
			var engineSymb = Connector.resolver.FindClass("Queen::QueenEngine");
			logicOffset = Connector.resolver.FieldOffset(engineSymb, "_logic");

			var logicClSymb = Connector.resolver.FindClass("Queen::Logic");
			currentRoomOffset = Connector.resolver.FieldOffset(logicClSymb, "_currentRoom");
			gameStateOffset = Connector.resolver.FieldOffset(logicClSymb, "_gameState");
		}

		public QueenState GetState() {
			var state = new QueenState();

			var logicPtrVal = Connector.memoryReader.ReadUInt32(EngineAddr + logicOffset);

			string logicName=Connector.rttiReader.GetMangledClassNameFromObjPtr(logicPtrVal);
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

			state.CurrentRoom = Connector.memoryReader.ReadUInt16(logicPtrVal + currentRoomOffset);
			state.GameState = Connector.memoryReader.ReadInt16Array(logicPtrVal + gameStateOffset, gameStateCount);

			return state;
		}
	}

	public class QueenState {
		public uint CurrentRoom;
		public short[] GameState;
		public LogicMode LogicMode;
	}

	public enum LogicMode {
		Game,
		Demo,
		Interview
	}
}
