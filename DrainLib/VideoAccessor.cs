using DrainLib.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib {
	internal class VideoAccessor : BaseAccessor {

		#region Symbol data

		//Decoder
		private int smkDecFileStreamOffset;
		private int videoDecNextVideoTrackOffset;
		//SMK Video track
		private int smkTrackCurFrameOffset;
		private int smkTrackFrameCountOffset;
		private int smkTrackFrameRateOffset;
		#endregion

		public VideoAccessor(ScummVMConnector connector) : base(connector) {
			LoadSymbols();
		}

		private void LoadSymbols() {
			var smkDecClSymb = Resolver.FindClass("Video::SmackerDecoder");
			smkDecFileStreamOffset = Resolver.FieldOffset(smkDecClSymb, "_fileStream");

			var videoDecClSymb = Resolver.FindClass("Video::VideoDecoder");
			videoDecNextVideoTrackOffset = Resolver.FieldOffset(videoDecClSymb, "_nextVideoTrack");

			var smkVideoTrackClSymb = Resolver.FindNestedClass(smkDecClSymb, "SmackerVideoTrack");
			smkTrackCurFrameOffset = Resolver.FieldOffset(smkVideoTrackClSymb, "_curFrame");
			smkTrackFrameCountOffset = Resolver.FieldOffset(smkVideoTrackClSymb, "_frameCount");
			smkTrackFrameRateOffset = Resolver.FieldOffset(smkVideoTrackClSymb, "_frameRate");
		}

		public VideoState ReadDecoder(IntPtr decoderAddr) {
			if(decoderAddr == IntPtr.Zero) return null;

			if(RttiReader.HasBaseClass(decoderAddr, ".?AVSmackerDecoder@Video@@")) {
				return ReadSmkDecoder(decoderAddr);
			}
			throw new NotSupportedException("No support for the decoder");
		}

		private VideoState ReadSmkDecoder(IntPtr decoderAddr) {
			var videoTrackPtrVal = MemoryReader.ReadIntPtr(decoderAddr + videoDecNextVideoTrackOffset);
			var fileStreamPtrVal = MemoryReader.ReadIntPtr(decoderAddr + smkDecFileStreamOffset);

			var state=ReadSmkVideoTrack(videoTrackPtrVal);
			state.FileName = ReadFileName(fileStreamPtrVal);
			return state;
		}

		private VideoState ReadSmkVideoTrack(IntPtr videoTrackAddr) {
			var state = new VideoState();

			state.CurrentFrame = MemoryReader.ReadUInt32(videoTrackAddr + smkTrackCurFrameOffset);
			state.FrameCount = (uint)MemoryReader.ReadInt32(videoTrackAddr + smkTrackFrameCountOffset);
			state.FrameRate = (float)ReadRational(videoTrackAddr + smkTrackFrameRateOffset);

			return state;
		}
	}
}
