using System;
using System.Collections.Generic;
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
		#endregion


		public BladeRunnerEnginerAccessor(ScummVMConnector connector, uint engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => throw new NotImplementedException();

		internal override void LoadSymbols() {
			var engSymb=Connector.resolver.FindClass("BladeRunner::BladeRunnerEngine");
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
			actorCluesCluesOffset = Connector.resolver.FieldOffset(actorCluesSymb,"_clues");
			actorCluesCountOffset = Connector.resolver.FieldOffset(actorCluesSymb, "_count");

			var clueSymb = Connector.resolver.FindNestedClass(actorCluesSymb, "Clue");
		}
	}
}
