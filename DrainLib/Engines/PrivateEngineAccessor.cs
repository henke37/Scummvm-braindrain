using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class PrivateEngineAccessor : ADBaseEngineAccessor {

		#region symbol data
		private int videoDecoderOffset;
		#endregion

		private VideoAccessor videoAccessor;

		public PrivateEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			videoAccessor = new VideoAccessor(Connector);
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Private::PrivateEngine");
			var descOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			videoDecoderOffset = Resolver.FieldOffset(engineCl, "_videoDecoder");

			LoadADSymbols(descOffset, true);
		}

		public override VideoState? GetVideoState() {
			var decoderAddr = MemoryReader.ReadIntPtr(EngineAddr + videoDecoderOffset);
			if(decoderAddr==IntPtr.Zero) {
				return null;
			}
			return videoAccessor.ReadDecoder(decoderAddr);
		}
	}
}
