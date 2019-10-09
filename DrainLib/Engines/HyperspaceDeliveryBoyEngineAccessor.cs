using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class HyperspaceDeliveryBoyEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		int gameStateOffset;
		int actionModeOffset;
		int currentMapnameOffset;
		const int currentMapnameSize=64;
		#endregion

		public HyperspaceDeliveryBoyEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "hbd";

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("HDB::HDBGame");
			gameStateOffset = Connector.resolver.FieldOffset(engineCl, "_gameState");
			actionModeOffset = Connector.resolver.FieldOffset(engineCl, "_actionMode");
			currentMapnameOffset = Connector.resolver.FieldOffset(engineCl, "_currentMapname");
		}

		public HBDState GetState() {
			var state = new HBDState();
			state.State = (GameState)Connector.memoryReader.ReadInt32(EngineAddr + gameStateOffset);
			state.ActionMode = 1==Connector.memoryReader.ReadInt32(EngineAddr + actionModeOffset);
			state.CurrentMap = Connector.memoryReader.ReadNullTermString(EngineAddr + currentMapnameOffset);
			return state;
		}

		public class HBDState {
			public string CurrentMap;
			public bool ActionMode;
			public GameState State;
		}

		public enum GameState {
			Title,
			Menu,
			Play,
			Loading
		}
	}
}
