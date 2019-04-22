using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public abstract class ADBaseEngineAccessor : BaseEngineAccessor {
		private uint adGameIdOffset;
		private uint adExtraOffset;
		private uint adFlagsOffset;

		private uint descOffset;
		private bool descIsPointer;

		internal ADBaseEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		protected void LoadADSymbols(uint descOffset, bool descIsPointer) {
			this.descOffset = descOffset;
			this.descIsPointer = descIsPointer;

			var descSymb = Connector.resolver.FindClass("ADGameDescription");
			adGameIdOffset = Connector.resolver.FieldOffset(descSymb, "gameId");
			adExtraOffset = Connector.resolver.FieldOffset(descSymb, "extra");
			adFlagsOffset = Connector.resolver.FieldOffset(descSymb, "flags");
		}

		public override string GameId => GetGameDescriptor().GameId;

		protected ADGameDescriptor GetGameDescriptor() {
			var addr = EngineAddr + descOffset;
			if(descIsPointer) {
				addr = Connector.memoryReader.ReadUInt32(addr);
			}

			var gameDesc = new ADGameDescriptor();

			uint gameIdVal = Connector.memoryReader.ReadUInt32(addr + adGameIdOffset);
			gameDesc.GameId = gameIdVal == 0 ? "" : Connector.memoryReader.ReadNullTermString(gameIdVal);
			uint extraVal = Connector.memoryReader.ReadUInt32(addr + adExtraOffset);
			gameDesc.Extra = extraVal == 0 ? "" : Connector.memoryReader.ReadNullTermString(extraVal);
			uint flagsVal = Connector.memoryReader.ReadUInt32(addr + adFlagsOffset);
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
