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

		public SciEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
		}

		internal override void LoadSymbols() {
			var engineCl = Connector.resolver.FindClass("Sci::SciEngine");
			video32Offset = Connector.resolver.FieldOffset(engineCl, "_video32");

			var video32Cl = Connector.resolver.FindClass("Sci::Video32");
			seqPlayerOffset = Connector.resolver.FieldOffset(video32Cl, "_SEQPlayer");
			aviPlayerOffset = Connector.resolver.FieldOffset(video32Cl, "_AVIPlayer");
			vmdPlayerOffset = Connector.resolver.FieldOffset(video32Cl, "_VMDPlayer");
			duckPlayerOffset = Connector.resolver.FieldOffset(video32Cl, "_duckPlayer");

			var videoPlayerCl = Connector.resolver.FindClass("Sci::VideoPlayer");
			var decoderField = Connector.resolver.FindField(videoPlayerCl, "_decoder");
			videoPlayerDecoderOffset = decoderField.offset;
			videoPlayerDecoderPointerOffset=Connector.resolver.FieldOffset(decoderField.type,"_pointer");

			var gameDescriptionOffset = Connector.resolver.FieldOffset(engineCl, "_gameDescription");
			LoadADSymbols(gameDescriptionOffset, true);
		}

		public override VideoState GetVideoState() {
			var video32PtrVal = Connector.memoryReader.ReadIntPtr(EngineAddr + video32Offset);
			if(video32PtrVal == IntPtr.Zero) return null;

			VideoState videoState;
			
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

		private VideoState ReadVideoPlayer(IntPtr playerAddr) {
			var decoderPtrAddr = playerAddr + videoPlayerDecoderOffset + videoPlayerDecoderPointerOffset;
			var decoderPtrVal = Connector.memoryReader.ReadIntPtr(decoderPtrAddr);
			if(decoderPtrVal == IntPtr.Zero) return null;

			return null;
		}
	}
}
