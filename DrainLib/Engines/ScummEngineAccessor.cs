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
		private uint smushActiveOffset;
		private uint smushPlayerOffset;

		private uint smushPlayerNBFramesOffset;
		private uint smushPlayerFrameOffset;
		private uint smushPlayerSpeedOffset;
		private uint smushPlayerSeekFileOffset;

		public readonly GameSettings GameSettings;

		internal ScummEngineAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
			GameSettings = GetGameSettings();

			if(GameSettings.Version>=7) {
				LoadSmushSymbols();
			}
		}

		internal override void LoadSymbols() {
			var engineClSymb = Connector.resolver.FindClass("Scumm::ScummEngine");
			gameOffset = Connector.resolver.FieldOffset(engineClSymb, "_game");

			var gameSettingsSymb = Connector.resolver.FindClass("Scumm::GameSettings");
			gameIdOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "gameid");
			variantOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "variant");
			versionOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "version");
			heVersionOffset = Connector.resolver.FieldOffset(gameSettingsSymb, "heversion");
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

		private GameSettings GetGameSettings() {
			uint addr=EngineAddr+gameOffset;

			var settings = new GameSettings();
			var gameIdPtrVal = Connector.memoryReader.ReadUInt32(addr + gameIdOffset);
			settings.GameId = Connector.memoryReader.ReadNullTermString(gameIdPtrVal);
			var variantPtrVal = Connector.memoryReader.ReadUInt32(addr + variantOffset);
			settings.Variant = Connector.memoryReader.ReadNullTermString(variantPtrVal);
			settings.Version = Connector.memoryReader.ReadByte(addr + versionOffset);
			settings.HeVersion = Connector.memoryReader.ReadByte(addr + heVersionOffset);
			return settings;
		}

		public SmushState GetSmushState() {
			if(GameSettings.Version < 7) return null;

			var active = Connector.memoryReader.ReadByte(EngineAddr + smushActiveOffset) != 0;
			if(!active) return null;

			var addr = Connector.memoryReader.ReadUInt32(EngineAddr + smushPlayerOffset);

			var state = new SmushState();
			state.CurrentFrame = Connector.memoryReader.ReadUInt32(addr + smushPlayerFrameOffset);
			state.FrameCount = Connector.memoryReader.ReadUInt32(addr + smushPlayerNBFramesOffset);
			state.FrameRate = Connector.memoryReader.ReadInt32(addr + smushPlayerSpeedOffset);
			state.File = ReadComString(addr + smushPlayerSeekFileOffset);
			return state;
		}
	}

	public class SmushState {
		public uint CurrentFrame;
		public uint FrameCount;
		public int FrameRate;
		public string File;
	}

	public class GameSettings {
		public string GameId;
		public string Variant;
		public int Version;
		public int HeVersion;
	}
}
