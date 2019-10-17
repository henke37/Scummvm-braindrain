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
			IntPtr infoPtrVal = MemoryReader.ReadIntPtr(infoPtrAddr);

			Debug.Assert(infoPtrVal != IntPtr.Zero);

			var info = new GameInfo();
			info.ActorCount = MemoryReader.ReadUInt32(infoPtrVal + infoActorCountOffset);
			info.PlayerId = MemoryReader.ReadUInt32(infoPtrVal + infoPlayerIdOffset);
			info.FlagCount = MemoryReader.ReadUInt32(infoPtrVal + infoFlagCountOffset);
			info.ClueCount = MemoryReader.ReadUInt32(infoPtrVal + infoClueCountOffset);
			info.GlobalVarCount = MemoryReader.ReadUInt32(infoPtrVal + infoGlobalVarCountOffset);
			info.CrimeCount = MemoryReader.ReadUInt32(infoPtrVal + infoCrimeCountOffset);
			info.SuspectCount = MemoryReader.ReadUInt32(infoPtrVal + infoSuspectCountOffset);
			return info;
		}

		public override string GameId => "bladerunner";

		internal override void LoadSymbols() {
			var engSymb = Resolver.FindClass("BladeRunner::BladeRunnerEngine");
			actorsOffset = Resolver.FieldOffset(engSymb, "_actors");
			gameVarsOffset = Resolver.FieldOffset(engSymb, "_gameVars");
			gameFlagsOffset = Resolver.FieldOffset(engSymb, "_gameFlags");
			gameInfoOffset = Resolver.FieldOffset(engSymb, "_gameInfo");
			cutContentOffset = Resolver.FieldOffset(engSymb, "_cutContent");

			var actorSymb = Resolver.FindClass("BladeRunner::Actor");
			actorHonestyOffset = Resolver.FieldOffset(actorSymb, "_honesty");
			actorIntelligenceOffset = Resolver.FieldOffset(actorSymb, "_intelligence");
			actorStabilityOffset = Resolver.FieldOffset(actorSymb, "_stability");
			actorGoalNumberOffset = Resolver.FieldOffset(actorSymb, "_goalNumber");
			actorFriendlinessOffset = Resolver.FieldOffset(actorSymb, "_friendlinessToOther");
			actorCurrentHpOffset = Resolver.FieldOffset(actorSymb, "_currentHP");
			actorMaxHpOffset = Resolver.FieldOffset(actorSymb, "_maxHP");
			actorCluesOffset = Resolver.FieldOffset(actorSymb, "_clues");

			var actorCluesSymb = Resolver.FindClass("BladeRunner::ActorClues");
			actorCluesCluesOffset = Resolver.FieldOffset(actorCluesSymb, "_clues");
			actorCluesCountOffset = Resolver.FieldOffset(actorCluesSymb, "_count");

			var clueSymb = Resolver.FindNestedClass(actorCluesSymb, "Clue");

			var flagsSymb = Resolver.FindClass("BladeRunner::GameFlags");
			flagsFlagsOffset = Resolver.FieldOffset(flagsSymb, "_flags");
			flagsFlagCountOffset = Resolver.FieldOffset(flagsSymb, "_flagCount");

			LoadGameInfoSymbols();
		}

		private void LoadGameInfoSymbols() {
			var infoSymb = Resolver.FindClass("BladeRunner::GameInfo");
			infoActorCountOffset = Resolver.FieldOffset(infoSymb, "_actorCount");
			infoPlayerIdOffset = Resolver.FieldOffset(infoSymb, "_playerId");
			infoFlagCountOffset = Resolver.FieldOffset(infoSymb, "_flagCount");
			infoClueCountOffset = Resolver.FieldOffset(infoSymb, "_clueCount");
			infoGlobalVarCountOffset = Resolver.FieldOffset(infoSymb, "_globalVarCount");
			infoCrimeCountOffset = Resolver.FieldOffset(infoSymb, "_crimeCount");
			infoSuspectCountOffset = Resolver.FieldOffset(infoSymb, "_suspectCount");
		}

		public GameState GetState() {
			var state = new GameState();
			state.CutContent = MemoryReader.ReadByte(EngineAddr+cutContentOffset) != 0;
			state.flagData = readGameFlagData();
			return state;
		}

		private byte[] readGameFlagData() {
			IntPtr flagsObjPtrAddr = EngineAddr + gameFlagsOffset;
			IntPtr flagsObjAddr = MemoryReader.ReadIntPtr(flagsObjPtrAddr);
			uint flagCount = MemoryReader.ReadUInt32(flagsObjAddr + flagsFlagCountOffset);
			IntPtr flagsArrPtrVal = MemoryReader.ReadIntPtr(flagsObjAddr + flagsFlagsOffset);
			return MemoryReader.ReadBytes(flagsArrPtrVal, flagCount / 8 + 1);
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
