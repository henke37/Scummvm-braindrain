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
		private int aviStreamHeadScaleOffset;
		private int aviStreamHeadRateOffset;
		//VMD Decoder
		private int vmdAdvancedDecoderDecoderOffset;
		private int coktelDecoderCurFrameOffset;
		private int coktelDecoderFrameCountOffset;
		private int coktelDecoderFrameRateOffset;
		private int vmdDecoderStreamOffset;
		//Bink decoder
		private int binkDecoderBinkOffset;
		private int binkTrackFrameRateOffset;
		private int binkTrackCurFrameOffset;
		private int binkTrackFrameCountOffset;
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
			aviTrackVidsHeaderOffset = Resolver.FieldOffset(aviTrackCl, "_vidsHeader");

			var streamHeadCl = Resolver.FindNestedClass(aviDecoderCl, "AVIStreamHeader");
			aviStreamHeadScaleOffset = Resolver.FieldOffset(streamHeadCl, "scale");
			aviStreamHeadRateOffset = Resolver.FieldOffset(streamHeadCl, "rate");

			var vmdAdvancedDecoderCl = Resolver.FindClass("Video::AdvancedVMDDecoder");
			vmdAdvancedDecoderDecoderOffset = Resolver.FieldOffset(vmdAdvancedDecoderCl, "_decoder");
			var coktelDecoderCl = Resolver.FindClass("Video::CoktelDecoder");
			coktelDecoderCurFrameOffset = Resolver.FieldOffset(coktelDecoderCl, "_curFrame");
			coktelDecoderFrameCountOffset = Resolver.FieldOffset(coktelDecoderCl, "_frameCount");
			coktelDecoderFrameRateOffset = Resolver.FieldOffset(coktelDecoderCl, "_frameRate");
			var vmdDecoderCl = Resolver.FindClass("Video::VMDDecoder");
			vmdDecoderStreamOffset = Resolver.FieldOffset(vmdDecoderCl, "_stream");

			var binkDecoderCl = Resolver.FindClass("Video::BinkDecoder");
			binkDecoderBinkOffset = Resolver.FieldOffset(binkDecoderCl, "_bink");

			var binkTrackCl = Resolver.FindClass("Video::BinkDecoder::BinkVideoTrack");
			binkTrackFrameRateOffset = Resolver.FieldOffset(binkTrackCl, "_frameRate");
			binkTrackCurFrameOffset = Resolver.FieldOffset(binkTrackCl, "_curFrame");
			binkTrackFrameCountOffset = Resolver.FieldOffset(binkTrackCl, "_frameCount");
		}

		public VideoState? ReadDecoder(IntPtr decoderAddr) {
			if(decoderAddr == IntPtr.Zero) return null;

			if(RttiReader.TryDynamicCast(decoderAddr, ".?AVSmackerDecoder@Video@@", out IntPtr smkDecoderAddr)) {
				return ReadSmkDecoder(smkDecoderAddr);
			}
			if(RttiReader.TryDynamicCast(decoderAddr, ".?AVAVIDecoder@Video@@", out IntPtr aviDecoderAddr)) {
				return ReadAviDecoder(aviDecoderAddr);
			}
			if(RttiReader.TryDynamicCast(decoderAddr, ".?AVAdvancedVMDDecoder@Video@@", out IntPtr vmdDecoderAddr)) {
				return ReadVmdDecoder(vmdDecoderAddr);
			}
			if(RttiReader.TryDynamicCast(decoderAddr, ".?AVBinkDecoder@Video@@", out IntPtr bnkDecoderAddr)) {
				return ReadBinkDecoder(bnkDecoderAddr);
			}
			throw new NotSupportedException("No support for the decoder");
		}

		private VideoState? ReadBinkDecoder(IntPtr decoderAddr) {
			var videoTrackPtrVal = MemoryReader.ReadIntPtr(decoderAddr + videoDecNextVideoTrackOffset);
			var fileStreamPtrVal = MemoryReader.ReadIntPtr(decoderAddr + binkDecoderBinkOffset);

			if(videoTrackPtrVal == IntPtr.Zero) return null;

			var state = ReadBinkVideoTrack(videoTrackPtrVal);
			state.FileName = ReadFileName(fileStreamPtrVal);
			return state;
		}

		private VideoState ReadBinkVideoTrack(IntPtr videoTrackAddr) {
			var state = new VideoState();
			state.CurrentFrame = MemoryReader.ReadUInt32(videoTrackAddr + binkTrackCurFrameOffset);
			state.FrameCount = MemoryReader.ReadUInt32(videoTrackAddr + binkTrackFrameCountOffset);
			state.FrameRate = ReadRational(videoTrackAddr + binkTrackFrameRateOffset);

			return state;
		}

		private VideoState? ReadSmkDecoder(IntPtr decoderAddr) {
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
			state.FrameRate = ReadRational(videoTrackAddr + smkTrackFrameRateOffset);

			return state;
		}

		private VideoState? ReadAviDecoder(IntPtr decoderAddr) {
			var videoTrackPtrVal = MemoryReader.ReadIntPtr(decoderAddr + videoDecNextVideoTrackOffset);

			if(videoTrackPtrVal == IntPtr.Zero) return null;

			var curFrame = MemoryReader.ReadInt32(videoTrackPtrVal + aviTrackCurFrameOffset);
			if(curFrame == -1) return null;

			var streamHeaderAddr =videoTrackPtrVal+aviTrackVidsHeaderOffset;

			var state = new VideoState();
			state.CurrentFrame = (uint)curFrame;
			state.FrameCount = MemoryReader.ReadUInt32(videoTrackPtrVal + aviTrackFrameCountOffset);
			var fileStreamPtrVal = MemoryReader.ReadIntPtr(decoderAddr + aviDecFileStreamOffset);
			state.FileName = ReadFileName(fileStreamPtrVal);

			state.FrameRate = new Rational() {
				Numerator=(int)MemoryReader.ReadUInt32(streamHeaderAddr+aviStreamHeadRateOffset),
				Denominator=(int)MemoryReader.ReadUInt32(streamHeaderAddr+aviStreamHeadScaleOffset)
			};

			return state;
		}

		private VideoState? ReadVmdDecoder(IntPtr decoderAddr) {
			var coktelDecoderAddr = MemoryReader.ReadIntPtr(decoderAddr + vmdAdvancedDecoderDecoderOffset);

			var curFrame= MemoryReader.ReadInt32(coktelDecoderAddr + coktelDecoderCurFrameOffset);
			if(curFrame <= -1) return null;

			var state = new VideoState();
			state.CurrentFrame = (uint)curFrame;
			state.FrameCount = MemoryReader.ReadUInt32(coktelDecoderAddr + coktelDecoderFrameCountOffset);
			state.FrameRate = ReadRational(coktelDecoderAddr + coktelDecoderFrameRateOffset);

			if(RttiReader.TryDynamicCast(coktelDecoderAddr, ".?AVVMDDecoder@Video@@",out IntPtr vmdDecoderAddr)) {
				var streamPtrVal = MemoryReader.ReadIntPtr(vmdDecoderAddr + vmdDecoderStreamOffset);
				state.FileName = ReadFileName(streamPtrVal);
			}

			return state;
		}
	}
}
