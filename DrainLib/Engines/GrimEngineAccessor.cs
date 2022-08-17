using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class GrimEngineAccessor : BaseEngineAccessor {

		#region Symbol data
		private int gameTypeOffset;
		private int gameFlagsOffset;
		#endregion

		public GrimEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			
		}

		public override string GameId => gameType == GameType.MONKEY4 ? "monkey4":"grim";
		public override bool IsDemo => (gameFlags & ADBaseEngineAccessor.GameFlags.Demo) != 0;

		private GameType gameType {
			get {
				return (GameType)MemoryReader.ReadUInt32(EngineAddr + gameTypeOffset);
			}
		}
		private ADBaseEngineAccessor.GameFlags gameFlags {
			get {
				return (ADBaseEngineAccessor.GameFlags)MemoryReader.ReadUInt32(EngineAddr + gameFlagsOffset);
			}
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Grim::GrimEngine");
			gameTypeOffset = Resolver.FieldOffset(engineCl, "_gameType");
			gameFlagsOffset = Resolver.FieldOffset(engineCl, "_gameFlags");
		}

		private enum GameType {
			GRIM=0,
			MONKEY4=1
		}

	}
}
