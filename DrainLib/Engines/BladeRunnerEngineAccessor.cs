using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class BladeRunnerEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		//Engine
		private int actorsOffset;
		private int gameVarsOffset;
		private int gameFlagsOffset;
		private int gameInfoOffset;
		private int cutContentOffset;

		//Actor
		private int actorHonestyOffset;
		private int actorIntelligenceOffset;
		private int actorStabilityOffset;
		private int actorGoalNumberOffset;
		private int actorFriendlinessOffset;
		private int actorCurrentHpOffset;
		private int actorMaxHpOffset;
		private int actorCluesOffset;

		//ActorClues
		private int actorCluesCluesOffset;
		private int actorCluesCountOffset;

		//GameInfo
		private int infoActorCountOffset;
		private int infoPlayerIdOffset;
		private int infoFlagCountOffset;
		private int infoClueCountOffset;
		private int infoGlobalVarCountOffset;
		private int infoCrimeCountOffset;
		private int infoSuspectCountOffset;

		//GameFlags
		private int flagsFlagsOffset;
		private int flagsFlagCountOffset;
		#endregion


		private readonly GameInfo gameInfo;


		public BladeRunnerEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			gameInfo = GetGameInfo();
		}

		private GameInfo GetGameInfo() {
			IntPtr infoPtrAddr = EngineAddr + gameInfoOffset;
			IntPtr infoPtrVal = Connector.memoryReader.ReadIntPtr(infoPtrAddr);

			Debug.Assert(infoPtrVal != IntPtr.Zero);

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

		public override string GameId => "bladerunner";

		internal override void LoadSymbols() {
			var engSymb = Connector.resolver.FindClass("BladeRunner::BladeRunnerEngine");
			actorsOffset = Connector.resolver.FieldOffset(engSymb, "_actors");
			gameVarsOffset = Connector.resolver.FieldOffset(engSymb, "_gameVars");
			gameFlagsOffset = Connector.resolver.FieldOffset(engSymb, "_gameFlags");
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

			var flagsSymb = Connector.resolver.FindClass("BladeRunner::GameFlags");
			flagsFlagsOffset = Connector.resolver.FieldOffset(flagsSymb, "_flags");
			flagsFlagCountOffset = Connector.resolver.FieldOffset(flagsSymb, "_flagCount");

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
			state.flagData = readGameFlagData();
			return state;
		}

		private byte[] readGameFlagData() {
			IntPtr flagsObjPtrAddr = EngineAddr + gameFlagsOffset;
			IntPtr flagsObjAddr = Connector.memoryReader.ReadIntPtr(flagsObjPtrAddr);
			uint flagCount = Connector.memoryReader.ReadUInt32(flagsObjAddr + flagsFlagCountOffset);
			IntPtr flagsArrPtrVal = Connector.memoryReader.ReadIntPtr(flagsObjAddr + flagsFlagsOffset);
			return Connector.memoryReader.ReadBytes(flagsArrPtrVal, flagCount / 8 + 1);
		}

		public class GameState {
			public bool CutContent;
			internal byte[] flagData;
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
