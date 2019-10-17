using System;
using System.Collections.Generic;

namespace DrainLib.Engines {
	public abstract class ADBaseEngineAccessor : BaseEngineAccessor {
		private int adGameIdOffset;
		private int adExtraOffset;
		private int adFlagsOffset;

		private int descOffset;
		private bool descIsPointer;

		internal ADBaseEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		protected void LoadADSymbols(int descOffset, bool descIsPointer) {
			this.descOffset = descOffset;
			this.descIsPointer = descIsPointer;

			var descSymb = Resolver.FindClass("ADGameDescription");
			adGameIdOffset = Resolver.FieldOffset(descSymb, "gameId");
			adExtraOffset = Resolver.FieldOffset(descSymb, "extra");
			adFlagsOffset = Resolver.FieldOffset(descSymb, "flags");
		}

		public override string GameId => GetGameDescriptor().GameId;

		protected ADGameDescriptor GetGameDescriptor() {
			var addr = EngineAddr + descOffset;
			if(descIsPointer) {
				addr = MemoryReader.ReadIntPtr(addr);
			}

			var gameDesc = new ADGameDescriptor();

			IntPtr gameIdVal = MemoryReader.ReadIntPtr(addr + adGameIdOffset);
			gameDesc.GameId = gameIdVal == IntPtr.Zero ? "" : MemoryReader.ReadNullTermString(gameIdVal);
			IntPtr extraVal = MemoryReader.ReadIntPtr(addr + adExtraOffset);
			gameDesc.Extra = extraVal == IntPtr.Zero ? "" : MemoryReader.ReadNullTermString(extraVal);
			uint flagsVal = MemoryReader.ReadUInt32(addr + adFlagsOffset);
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
