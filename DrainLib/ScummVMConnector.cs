
using DebugHelp;
using DrainLib.Engines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace DrainLib {
	public class ScummVMConnector {

		private Process process;
		private LiveProcessMemoryReader memoryReader;
		private SymbolResolver resolver;

		private uint g_engineAddr;

		public ScummVMConnector() { }

		public void Connect() {
			var procs = Process.GetProcessesByName("scummvm");
			if(procs.Length == 0) throw new ProcessNotFoundException();

			process = procs[0];
			memoryReader = new LiveProcessMemoryReader(process);

			string pdbPath = process.MainModule.FileName.Replace(".exe", ".pdb");
			resolver = new SymbolResolver(pdbPath);

			var g_engineSymb = resolver.findGlobal("g_engine");
			g_engineAddr = g_engineSymb.addressOffset;
		}

		public BaseEngineAccessor Engine {
			get => getEngine();
		}

		private BaseEngineAccessor getEngine() {
			var enginePtrVal=memoryReader.ReadUInt32At((uint)process.MainModule.BaseAddress + g_engineAddr);

			if(enginePtrVal == 0) return null;

			throw new NotImplementedException();
		}
	}

	[Serializable]
	public class ProcessNotFoundException : Exception {
		internal ProcessNotFoundException() : base("Target process not found") {
			
		}
		protected ProcessNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}
