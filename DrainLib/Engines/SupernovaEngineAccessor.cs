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
			var engineCl=this.Resolver.FindClass("Supernova::SupernovaEngine");
			msPartOffset = Resolver.FieldOffset(engineCl,"_MSPart");
			gmOffset = Resolver.FieldOffset(engineCl, "_gm");

			var gameManagerCl = Resolver.FindClass("Supernova::GameManager");
			currentRoomOffset = Resolver.FieldOffset(gameManagerCl, "_currentRoom");

			var roomCl = Resolver.FindClass("Supernova::Room");
			roomIdOffset = Resolver.FieldOffset(roomCl, "_id");
		}

		public int Part {
			get {
				return MemoryReader.ReadByte(EngineAddr + msPartOffset);
			}
		}

		public int CurrentRoomId {
			get {
				IntPtr gmAddr = MemoryReader.ReadIntPtr(EngineAddr + gmOffset);
				IntPtr roomAddr = MemoryReader.ReadIntPtr(gmAddr + currentRoomOffset);
				return MemoryReader.ReadInt32(roomAddr + roomIdOffset);
			}
		}
	}
}
