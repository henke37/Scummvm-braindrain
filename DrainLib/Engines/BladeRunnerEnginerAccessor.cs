using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class BladeRunnerEnginerAccessor : BaseEngineAccessor {

		#region Symbol data
		//Engine
		private uint actorsOffset;
		private uint gameVarsOffset;
		private uint gameInfoOffset;
		private uint cutContentOffset;

		//Actor
		private uint actorHonestyOffset;
		private uint actorIntelligenceOffset;
		private uint actorStabilityOffset;
		private uint actorGoalNumberOffset;
		private uint actorFriendlinessOffset;
		private uint actorCurrentHpOffset;
		private uint actorMaxHpOffset;
		private uint actorCluesOffset;

		//ActorClues
		private uint actorCluesCluesOffset;
		private uint actorCluesCountOffset;

		//GameInfo
		private uint infoActorCountOffset;
		private uint infoPlayerIdOffset;
		private uint infoFlagCountOffset;
		private uint infoClueCountOffset;
		private uint infoGlobalVarCountOffset;
		private uint infoCrimeCountOffset;
		private uint infoSuspectCountOffset;
		#endregion


		private readonly GameInfo gameInfo;


		public BladeRunnerEnginerAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
			gameInfo = GetGameInfo();
		}

		private GameInfo GetGameInfo() {
			uint infoPtrAddr = EngineAddr + gameInfoOffset;
			uint infoPtrVal = Connector.memoryReader.ReadUInt32(infoPtrAddr);

			Debug.Assert(infoPtrVal != 0);

			var info = new GameInfo();
			info.ActorCount = Connector.memoryReader.ReadUInt32(infoPtrVal + infoActorCountOffset);
			info.PlayerId = Connector.memoryReader.ReadUInt32(infoPtrVal + infoPlayerIdOffset);
			info.FlagCount = Connector.memoryReader.ReadUInt32(infoPtrVal + infoFlagCountOffset);
			info.ClueCount = Connector.memoryReader.ReadUInt32(infoPtrVal + infoClueCountOffset);
			info.GlobalVarCount = Connector.memoryReader.ReadUInt32(infoPtrVal + infoGlobalVarCountOffset);
			info.CrimeCount = Connector.memoryReader.ReadUInt32(infoPtrVal + infoCrimeCountOffset);
			info.SuspectCount = Connector.memoryReader.ReadUInt32(infoPtrVal + infoSuspectCountOffset);
			return info;
		}

		public override string GameId => throw new NotImplementedException();

		internal override void LoadSymbols() {
			var engSymb = Connector.resolver.FindClass("BladeRunner::BladeRunnerEngine");
			actorsOffset = Connector.resolver.FieldOffset(engSymb, "_actors");
			gameVarsOffset = Connector.resolver.FieldOffset(engSymb, "_gameVars");
			gameInfoOffset = Connector.resolver.FieldOffset(engSymb, "_gameInfo");
			cutContentOffset = Connector.resolver.FieldOffset(engSymb, "_cutContent");

			var actorSymb = Connector.resolver.FindClass("BladeRunner::Actor");
			actorHonestyOffset = Connector.resolver.FieldOffset(actorSymb, "_honesty");
			actorIntelligenceOffset = Connector.resolver.FieldOffset(actorSymb, "_intelligence");
			actorStabilityOffset = Connector.resolver.FieldOffset(actorSymb, "_stability");
			actorGoalNumberOffset = Connector.resolver.FieldOffset(actorSymb, "_goalNumber");
			actorFriendlinessOffset = Connector.resolver.FieldOffset(actorSymb, "_friendlinessToOther");
			actorCurrentHpOffset = Connector.resolver.FieldOffset(actorSymb, "_currentHP");
			actorMaxHpOffset = Connector.resolver.FieldOffset(actorSymb, "_maxHP");
			actorCluesOffset = Connector.resolver.FieldOffset(actorSymb, "_clues");

			var actorCluesSymb = Connector.resolver.FindClass("BladeRunner::ActorClues");
			actorCluesCluesOffset = Connector.resolver.FieldOffset(actorCluesSymb, "_clues");
			actorCluesCountOffset = Connector.resolver.FieldOffset(actorCluesSymb, "_count");

			var clueSymb = Connector.resolver.FindNestedClass(actorCluesSymb, "Clue");
			LoadGameInfoSymbols();
		}

		private void LoadGameInfoSymbols() {
			var infoSymb = Connector.resolver.FindClass("BladeRunner::GameInfo");
			infoActorCountOffset = Connector.resolver.FieldOffset(infoSymb, "_actorCount");
			infoPlayerIdOffset = Connector.resolver.FieldOffset(infoSymb, "_playerId");
			infoFlagCountOffset = Connector.resolver.FieldOffset(infoSymb, "_flagCount");
			infoClueCountOffset = Connector.resolver.FieldOffset(infoSymb, "_clueCount");
			infoGlobalVarCountOffset = Connector.resolver.FieldOffset(infoSymb, "_globalVarCount");
			infoCrimeCountOffset = Connector.resolver.FieldOffset(infoSymb, "_crimeCount");
			infoSuspectCountOffset = Connector.resolver.FieldOffset(infoSymb, "_suspectCount");
		}

		public GameState GetState() {
			var state = new GameState();
			state.CutContent = Connector.memoryReader.ReadByte(EngineAddr+cutContentOffset) != 0;
			return state;
		}

		public class GameState {
			public bool CutContent;
		}

		private class GameInfo {
			internal uint ActorCount;
			internal uint PlayerId;
			internal uint FlagCount;
			internal uint ClueCount;
			internal uint GlobalVarCount;
			internal uint CrimeCount;
			internal uint SuspectCount;
		}
	}
}
