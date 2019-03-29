using System;
using System.Collections.Generic;
using DebugHelp;

namespace DrainLib.Engines {
	public class ScummEngineAccessor : BaseEngineAccessor {
		private uint gameIdOffset;
		private uint variantOffset;
		private uint versionOffset;
		private uint heVersionOffset;
		private uint gameOffset;

		public readonly GameSettings GameSettings;

		internal ScummEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
			GameSettings = GetGameSettings();
		}

		internal override void LoadSymbols() {
			var engineClSymb = Connector.resolver.FindClass("Scumm::ScummEngine");
			gameOffset = Connector.resolver.FieldOffset(engineClSymb,"_game");

			var gameSettingsSymb = Connector.resolver.FindClass("Scumm::GameSettings");
			gameIdOffset = Connector.resolver.FieldOffset(gameSettingsSymb,"gameId");
			variantOffset = Connector.resolver.FieldOffset(gameSettingsSymb,"variantId");
			variantOffset = Connector.resolver.FieldOffset(gameSettingsSymb,"variantId");
			versionOffset = Connector.resolver.FieldOffset(gameSettingsSymb,"version");
			heVersionOffset = Connector.resolver.FieldOffset(gameSettingsSymb,"heversion");
		}

		public override string GameId => GameSettings.GameId;

		private GameSettings GetGameSettings() {
			uint addr=Connector.memoryReader.ReadUInt32At(EngineAddr+gameOffset);

			var settings = new GameSettings();
			var gameIdPtrVal = Connector.memoryReader.ReadUInt32At(addr + gameIdOffset);
			settings.GameId = Connector.memoryReader.ReadNullTermString(gameIdPtrVal);
			var variantPtrVal = Connector.memoryReader.ReadUInt32At(addr + variantOffset);
			settings.Variant = Connector.memoryReader.ReadNullTermString(variantPtrVal);
			settings.Version = Connector.memoryReader.ReadByte(addr + versionOffset);
			settings.HeVersion = Connector.memoryReader.ReadByte(addr + heVersionOffset);
			return settings;
		}

		
	}

	public class GameSettings {
		public string GameId;
		public string Variant;
		public int Version;
		public int HeVersion;
	}
}
