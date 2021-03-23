using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class TinselEngineAccessor : ADBaseEngineAccessor {

		#region symbol data
		//Main engine class
		private int bmvOffset;

		//bmv player
		private int bmvp_movieOnOffset;
		private int bmvp_streamOffset;
		private int bmvp_currentFrameOffset;
		#endregion

		public TinselEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Tinsel::TinselEngine");
			var descOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			bmvOffset = Resolver.FieldOffset(engineCl, "_bmv");

			LoadADSymbols(descOffset, true);

			var bmvPlayerCl = Resolver.FindClass("Tinsel::BMVPlayer");
			bmvp_movieOnOffset = Resolver.FieldOffset(bmvPlayerCl, "bMovieOn");
			bmvp_streamOffset = Resolver.FieldOffset(bmvPlayerCl, "stream");
			bmvp_currentFrameOffset = Resolver.FieldOffset(bmvPlayerCl, "currentFrame");
		}

		public override VideoState? GetVideoState() {
			var bmvPtr=MemoryReader.ReadIntPtr(EngineAddr + bmvOffset);

			bool movieOn = MemoryReader.ReadByte(bmvPtr + bmvp_movieOnOffset) != 0;
			if(!movieOn) return null;

			var video = new VideoState();
			video.CurrentFrame = MemoryReader.ReadUInt32(bmvPtr + bmvp_currentFrameOffset);
			video.FileName = this.ReadFileName(bmvPtr + bmvp_streamOffset);
			video.FrameRate = new Rational(2, 24);

			return video;
		}
	}
}
