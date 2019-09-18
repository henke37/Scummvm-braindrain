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
		#endregion

		public SupernovaEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		public override string GameId => "supernova"+Part;

		internal override void LoadSymbols() {
			var engineCl=this.Connector.resolver.FindClass("Supernova::SupernovaEngine");
			msPartOffset = Connector.resolver.FieldOffset(engineCl,"_MSPart");
		}

		public int Part {
			get {
				return Connector.memoryReader.ReadByte(EngineAddr + msPartOffset);
			}
		}
	}
}
