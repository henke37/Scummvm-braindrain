using DebugHelp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DrainLib.Engines {
	public abstract class BaseEngineAccessor {
		protected ScummVMConnector Connector;

		protected uint EngineAddr;

		#region Symbol data
		//Engine
		private uint mainMenuDialogOffset;
		private uint guiVisibleOffset;
		private uint pauseLevelOffset;

		//Com::String
		private uint comStringSizeOffset;
		private uint comStringStrOffset;

		//Com::File
		private uint comFileNameOffset;
		#endregion

		internal BaseEngineAccessor(ScummVMConnector connector, uint engineAddr) {
			this.Connector = connector;
			this.EngineAddr = engineAddr;

			LoadBaseSymbols();
			LoadSymbols();
		}

		private void LoadBaseSymbols() {
			var engSymb=Connector.resolver.FindClass("Engine");

			mainMenuDialogOffset = Connector.resolver.FieldOffset(engSymb, "_mainMenuDialog");
			Debug.Assert(mainMenuDialogOffset != 0, "mainMenuDialogOffset==0");

			var guiDialogSymb = Connector.resolver.FindClass("GUI::Dialog");
			guiVisibleOffset = Connector.resolver.FieldOffset(guiDialogSymb, "_visible");
			Debug.Assert(guiVisibleOffset != 0, "guiVisibleOffset==0");
			var fieldSize = Connector.resolver.FieldSize(guiDialogSymb, "_visible");
			Debug.Assert(fieldSize == 1, "guiVisibleSize!=1");

			pauseLevelOffset = Connector.resolver.FieldOffset(engSymb, "_pauseLevel");

			var comStringSymb = Connector.resolver.FindClass("Common::String");
			comStringSizeOffset = Connector.resolver.FieldOffset(comStringSymb, "_size");
			comStringStrOffset = Connector.resolver.FieldOffset(comStringSymb, "_str");

			var comFileClSymb = Connector.resolver.FindClass("Common::File");
			comFileNameOffset = Connector.resolver.FieldOffset(comFileClSymb, "_name");
		}

		internal abstract void LoadSymbols();

		public bool IsActiveEngine {
			get {
				var liveEnginePtrVal=Connector.memoryReader.ReadUInt32(Connector.g_engineAddr);
				return liveEnginePtrVal == EngineAddr;
			}
		}

		public bool MainMenuOpen {
			get {
				uint mainMenuDialogPtrVal=Connector.memoryReader.ReadUInt32(EngineAddr + mainMenuDialogOffset);
				if(mainMenuDialogPtrVal == 0) return false;
				byte visibleVal = Connector.memoryReader.ReadByte(mainMenuDialogPtrVal + guiVisibleOffset);
				return visibleVal != 0;
			}
		}

		public bool Paused {
			get {
				int pauseLevel = Connector.memoryReader.ReadInt32(EngineAddr + pauseLevelOffset);
				return pauseLevel > 0;
			}
		}

		protected string ReadComString(uint addr) {
			uint size = Connector.memoryReader.ReadUInt32(addr + comStringSizeOffset);
			uint strPtr = Connector.memoryReader.ReadUInt32(addr + comStringStrOffset);

			var strBytes=Connector.memoryReader.ReadBytes(strPtr, size);
			return new string(Encoding.UTF8.GetChars(strBytes));
		}

		protected string ReadFileName(uint streamAddr) {
			if(!Connector.rttiReader.TryDynamicCast(streamAddr, ".?AVFile@Common@@", out var fileAddr)) return null;
			return ReadFileNameInternal(fileAddr);
		}
		protected string ReadFileNameInternal(uint fileAddr) {
			return ReadComString(fileAddr + comFileNameOffset);
		}

		public virtual VideoState GetVideoState() { return null; }

		public abstract string GameId { get; }
	}

	public class VideoState {
		public uint CurrentFrame;
		public uint FrameCount;
		public int FrameRate;
		public string FileName;
	}
}
