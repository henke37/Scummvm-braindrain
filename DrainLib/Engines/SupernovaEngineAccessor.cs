using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class SupernovaEngineAccessor : BaseEngineAccessor {

		#region symbol data
		//engine class
		private int msPartOffset;
		private int gmOffset;
		//GameManager class
		private int currentRoomOffset;
		//Room class
		private int roomIdOffset;
		#endregion

		public SupernovaEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "supernova"+Part;

		internal override void LoadSymbols() {
			var engineCl=this.Connector.resolver.FindClass("Supernova::SupernovaEngine");
			msPartOffset = Connector.resolver.FieldOffset(engineCl,"_MSPart");
			gmOffset = Connector.resolver.FieldOffset(engineCl, "_gm");

			var gameManagerCl = Connector.resolver.FindClass("Supernova::GameManager");
			currentRoomOffset = Connector.resolver.FieldOffset(gameManagerCl, "_currentRoom");

			var roomCl = Connector.resolver.FindClass("Supernova::Room");
			roomIdOffset = Connector.resolver.FieldOffset(roomCl, "_id");
		}

		public int Part {
			get {
				return Connector.memoryReader.ReadByte(EngineAddr + msPartOffset);
			}
		}

		public int CurrentRoomId {
			get {
				IntPtr gmAddr = Connector.memoryReader.ReadIntPtr(EngineAddr + gmOffset);
				IntPtr roomAddr = Connector.memoryReader.ReadIntPtr(gmAddr + currentRoomOffset);
				return Connector.memoryReader.ReadInt32(roomAddr + roomIdOffset);
			}
		}
	}
}
