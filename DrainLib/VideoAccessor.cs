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
		private int videoDecNextVideoTrackOffset;
		//SML Decoder
		private int smkDecFileStreamOffset;
		//SMK Video track
		private int smkTrackCurFrameOffset;
		private int smkTrackFrameCountOffset;
		private int smkTrackFrameRateOffset;
		//AVI Decoder
		private int aviDecFileStreamOffset;
		//AVI Video Track
		private int aviTrackCurFrameOffset;
		private int aviTrackFrameCountOffset;
		private int aviTrackVidsHeaderOffset;
		//VMD Decoder
		private int vmdDecoderDecoderOffset;
		private int coktelDecoderCurFrameOffset;
		private int coktelDecoderFrameCountOffset;
		private int coktelDecoderFrameRateOffset;
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

			var aviDecoderCl = Resolver.FindClass("Video::AVIDecoder");
			aviDecFileStreamOffset = Resolver.FieldOffset(aviDecoderCl, "_fileStream");

			var aviTrackCl = Resolver.FindNestedClass(aviDecoderCl, "AVIVideoTrack");
			aviTrackCurFrameOffset = Resolver.FieldOffset(aviTrackCl, "_curFrame");
			aviTrackFrameCountOffset = Resolver.FieldOffset(aviTrackCl, "_frameCount");

			var vmdDecoderCl = Resolver.FindClass("Video::AdvancedVMDDecoder");
			vmdDecoderDecoderOffset = Resolver.FieldOffset(vmdDecoderCl, "_decoder");
			var coktelDecoderCl = Resolver.FindClass("Video::CoktelDecoder");
			coktelDecoderCurFrameOffset = Resolver.FieldOffset(coktelDecoderCl, "_curFrame");
			coktelDecoderFrameCountOffset = Resolver.FieldOffset(coktelDecoderCl, "_frameCount");
			coktelDecoderFrameRateOffset = Resolver.FieldOffset(coktelDecoderCl, "_frameRate");
		}

		public VideoState ReadDecoder(IntPtr decoderAddr) {
			if(decoderAddr == IntPtr.Zero) return null;

			if(RttiReader.HasBaseClass(decoderAddr, ".?AVSmackerDecoder@Video@@")) {
				return ReadSmkDecoder(decoderAddr);
			}
			if(RttiReader.HasBaseClass(decoderAddr, ".?AVAVIDecoder@Video@@")) {
				return ReadAviDecoder(decoderAddr);
			}
			if(RttiReader.HasBaseClass(decoderAddr, ".?AVAdvancedVMDDecoder@Video@@")) {
				return ReadVmdDecoder(decoderAddr);
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

		private VideoState ReadAviDecoder(IntPtr decoderAddr) {
			var videoTrackPtrVal = MemoryReader.ReadIntPtr(decoderAddr + videoDecNextVideoTrackOffset);

			if(videoTrackPtrVal == IntPtr.Zero) return null;

			var curFrame = MemoryReader.ReadInt32(videoTrackPtrVal + aviTrackCurFrameOffset);
			if(curFrame == -1) return null;

			var state = new VideoState();
			state.CurrentFrame = (uint)curFrame;
			state.FrameCount = MemoryReader.ReadUInt32(videoTrackPtrVal + aviTrackFrameCountOffset);
			state.FileName = ReadFileName(decoderAddr + aviDecFileStreamOffset);

			return state;
		}

		private VideoState ReadVmdDecoder(IntPtr decoderAddr) {
			var coktelDecoderAddr = MemoryReader.ReadIntPtr(decoderAddr + vmdDecoderDecoderOffset);

			var curFrame= MemoryReader.ReadInt32(coktelDecoderAddr + coktelDecoderCurFrameOffset);
			if(curFrame <= -1) return null;

			var state = new VideoState();
			state.CurrentFrame = (uint)curFrame;
			state.FrameCount = MemoryReader.ReadUInt32(coktelDecoderAddr + coktelDecoderFrameCountOffset);
			state.FrameRate = (float)ReadRational(coktelDecoderAddr + coktelDecoderFrameRateOffset);
			return state;
		}
	}
}
