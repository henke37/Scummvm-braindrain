using Henke37.DebugHelp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DrainLib.Engines {
	public abstract class BaseEngineAccessor : BaseAccessor {

		protected IntPtr EngineAddr;

		#region Symbol data
		//Engine
		private int mainMenuDialogOffset;
		private int guiVisibleOffset;
		private int pauseLevelOffset;

		#endregion

		internal BaseEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector) {
			this.EngineAddr = engineAddr;

			LoadBaseEngineSymbols();
			LoadSymbols();
		}

		private void LoadBaseEngineSymbols() {
			var engSymb=Resolver.FindClass("Engine");

			mainMenuDialogOffset = Resolver.FieldOffset(engSymb, "_mainMenuDialog");
			Debug.Assert(mainMenuDialogOffset != 0, "mainMenuDialogOffset==0");

			var guiDialogSymb = Resolver.FindClass("GUI::Dialog");
			guiVisibleOffset = Resolver.FieldOffset(guiDialogSymb, "_visible");
			Debug.Assert(guiVisibleOffset != 0, "guiVisibleOffset==0");
			var fieldSize = Resolver.FieldSize(guiDialogSymb, "_visible");
			Debug.Assert(fieldSize == 1, "guiVisibleSize!=1");

			pauseLevelOffset = Resolver.FieldOffset(engSymb, "_pauseLevel");
		}

		internal abstract void LoadSymbols();

		public bool IsActiveEngine {
			get {
				var liveEnginePtrVal=MemoryReader.ReadIntPtr(Connector.g_engineAddr);
				return liveEnginePtrVal == EngineAddr;
			}
		}

		public bool MainMenuOpen {
			get {
				var mainMenuDialogPtrVal=MemoryReader.ReadIntPtr(EngineAddr + mainMenuDialogOffset);
				if(mainMenuDialogPtrVal == IntPtr.Zero) return false;
				byte visibleVal = MemoryReader.ReadByte(mainMenuDialogPtrVal + guiVisibleOffset);
				return visibleVal != 0;
			}
		}

		public bool Paused {
			get {
				int pauseLevel = MemoryReader.ReadInt32(EngineAddr + pauseLevelOffset);
				return pauseLevel > 0;
			}
		}

		public virtual VideoState? GetVideoState() { return null; }

		public abstract string GameId { get; }

		public virtual bool IsDemo => false;
	}
}
