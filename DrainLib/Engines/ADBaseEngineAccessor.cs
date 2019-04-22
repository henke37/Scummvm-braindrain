using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public abstract class ADBaseEngineAccessor : BaseEngineAccessor {
		private uint gameIdOffset;
		private uint extraOffset;
		private uint flagsOffset;

		private uint descOffset;
		private bool descIsPointer;

		internal ADBaseEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		protected void LoadADSymbols(uint descOffset, bool descIsPointer) {
			this.descOffset = descOffset;
			this.descIsPointer = descIsPointer;

			var descSymb = Connector.resolver.FindClass("ADGameDescription");
			gameIdOffset = Connector.resolver.FieldOffset(descSymb, "gameId");
			extraOffset = Connector.resolver.FieldOffset(descSymb, "extra");
			flagsOffset = Connector.resolver.FieldOffset(descSymb, "flags");
		}

		public override string GameId => GetGameDescriptor().GameId;

		protected ADGameDescriptor GetGameDescriptor() {
			var addr = EngineAddr + descOffset;
			if(descIsPointer) {
				addr = Connector.memoryReader.ReadUInt32(addr);
			}

			var gameDesc = new ADGameDescriptor();

			uint gameIdVal = Connector.memoryReader.ReadUInt32(addr + gameIdOffset);
			gameDesc.GameId = gameIdVal == 0 ? "" : Connector.memoryReader.ReadNullTermString(gameIdVal);
			uint extraVal = Connector.memoryReader.ReadUInt32(addr + extraOffset);
			gameDesc.Extra = extraVal == 0 ? "" : Connector.memoryReader.ReadNullTermString(extraVal);
			uint flagsVal = Connector.memoryReader.ReadUInt32(addr + flagsOffset);
			gameDesc.GameFlags = (GameFlags)flagsVal;

			return gameDesc;
		}

		public class ADGameDescriptor {
			public string GameId;
			public string Extra;

			public GameFlags GameFlags;
		}

		[Flags]
		public enum GameFlags {
			None = 0,
			AutoGenTarget = 1 << 20,
			Unstable = 1 << 21,
			Testing = 1 << 22,
			Pirated = 1 << 23,
			AddEnglish = 1 << 24,
			MacResFork = 1 << 25,
			UseExtraAsTitle = 1 << 26,
			DropLanguage = 1 << 27,
			DropPlatform = 1 << 28,
			CD = 1 << 29,
			Demo = 1 << 30
		}
	}
}
