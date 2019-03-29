using DebugHelp;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DrainLib.Engines {
	public abstract class BaseEngineAccessor {
		protected ScummVMConnector Connector;

		protected uint EngineAddr;

		private uint mainMenuDialogOffset;
		private uint guiVisibleOffset;

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
		}

		internal abstract void LoadSymbols();

		public bool IsActiveEngine {
			get {
				var liveEnginePtrVal=Connector.memoryReader.ReadUInt32At(Connector.g_engineAddr);
				return liveEnginePtrVal == EngineAddr;
			}
		}

		public bool MainMenuOpen {
			get {
				uint mainMenuDialogPtrVal=Connector.memoryReader.ReadUInt32At(EngineAddr + mainMenuDialogOffset);
				if(mainMenuDialogPtrVal == 0) return false;
				byte visibleVal = Connector.memoryReader.ReadByte(mainMenuDialogPtrVal + guiVisibleOffset);
				return visibleVal != 0;
			}
		}
	}
}
