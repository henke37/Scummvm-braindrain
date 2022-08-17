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
		private int engineModeOffset;

		private IntPtr gMovieAddr;
		private int videoDecoderOffset;
		#endregion

		private VideoAccessor videoAccessor;

		public GrimEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			videoAccessor = new VideoAccessor(connector);
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Grim::GrimEngine");
			gameTypeOffset = Resolver.FieldOffset(engineCl, "_gameType");
			gameFlagsOffset = Resolver.FieldOffset(engineCl, "_gameFlags");
			engineModeOffset = Resolver.FieldOffset(engineCl, "_mode");

			var movieCl = Resolver.FindClass("Grim::MoviePlayer");
			videoDecoderOffset = Resolver.FieldOffset(movieCl, "_videoDecoder");

			var gMovieSymb = Resolver.FindGlobal("Grim::g_movie");
			gMovieAddr = (IntPtr)gMovieSymb.virtualAddress;
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

		private EngineMode engineMode {
			get {
				return (EngineMode)MemoryReader.ReadInt32(EngineAddr + engineModeOffset);
			}
		}

		public override VideoState? GetVideoState() {
			var gVideoVal = MemoryReader.ReadIntPtr(gMovieAddr);

			var videoDecoderAddr = MemoryReader.ReadIntPtr(gVideoVal + videoDecoderOffset);

			return videoAccessor.ReadDecoder(videoDecoderAddr);
		}

		private enum GameType {
			GRIM=0,
			MONKEY4=1
		}

		private enum EngineMode {
			PauseMode = 1,
			NormalMode = 2,
			SmushMode = 3,
			DrawMode = 4,
			OverworldMode = 5
		}

	}
}
