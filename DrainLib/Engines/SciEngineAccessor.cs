using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class SciEngineAccessor : ADBaseEngineAccessor {

		#region Symbol data
		//engine class
		private int video32Offset;

		//Video32 class
		private int seqPlayerOffset;
		private int aviPlayerOffset;
		private int vmdPlayerOffset;
		private int duckPlayerOffset;

		//VideoPlayer class
		private int videoPlayerDecoderOffset;
		private int videoPlayerDecoderPointerOffset;
		//
		#endregion

		private VideoAccessor videoAccessor;

		public SciEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			videoAccessor = new VideoAccessor(connector);
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Sci::SciEngine");
			video32Offset = Resolver.FieldOffset(engineCl, "_video32");

			var video32Cl = Resolver.FindClass("Sci::Video32");
			seqPlayerOffset = Resolver.FieldOffset(video32Cl, "_SEQPlayer");
			aviPlayerOffset = Resolver.FieldOffset(video32Cl, "_AVIPlayer");
			vmdPlayerOffset = Resolver.FieldOffset(video32Cl, "_VMDPlayer");
			duckPlayerOffset = Resolver.FieldOffset(video32Cl, "_duckPlayer");

			var videoPlayerCl = Resolver.FindClass("Sci::VideoPlayer");
			var decoderField = Resolver.FindField(videoPlayerCl, "_decoder");
			videoPlayerDecoderOffset = decoderField.offset;
			videoPlayerDecoderPointerOffset=Resolver.FieldOffset(decoderField.type,"_pointer");

			var gameDescriptionOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			LoadADSymbols(gameDescriptionOffset, true);
		}

		public override VideoState? GetVideoState() {
			var video32PtrVal = MemoryReader.ReadIntPtr(EngineAddr + video32Offset);
			if(video32PtrVal == IntPtr.Zero) return null;

			VideoState? videoState;
			
			videoState = ReadVideoPlayer(video32PtrVal + seqPlayerOffset);
			if(videoState!=null) return videoState;
			videoState = ReadVideoPlayer(video32PtrVal + aviPlayerOffset);
			if(videoState != null) return videoState;
			videoState = ReadVideoPlayer(video32PtrVal + vmdPlayerOffset);
			if(videoState != null) return videoState;
			videoState = ReadVideoPlayer(video32PtrVal + duckPlayerOffset);
			if(videoState != null) return videoState;

			return null;
		}

		private VideoState? ReadVideoPlayer(IntPtr playerAddr) {
			var decoderPtrAddr = playerAddr + videoPlayerDecoderOffset + videoPlayerDecoderPointerOffset;
			var decoderPtrVal = MemoryReader.ReadIntPtr(decoderPtrAddr);
			if(decoderPtrVal == IntPtr.Zero) return null;

			return videoAccessor.ReadDecoder(decoderPtrVal);
		}
	}
}
