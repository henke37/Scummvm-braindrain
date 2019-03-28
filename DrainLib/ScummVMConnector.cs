
using DebugHelp;
using DebugHelp.RTTI;
using DrainLib.Engines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace DrainLib {
	public class ScummVMConnector {

		private Process process;
		internal ProcessMemoryReader memoryReader;
		internal RTTIReader rttiReader;
		internal SymbolResolver resolver;

		internal uint g_engineAddr;

		public ScummVMConnector() { }

		public void Connect() {
			var procs = Process.GetProcessesByName("scummvm");
			if(procs.Length == 0) throw new ProcessNotFoundException();

			process = procs[0];
			memoryReader = new LiveProcessMemoryReader(process);

			rttiReader = new RTTIReader(memoryReader);
			try {
				string pdbPath = process.MainModule.FileName.Replace(".exe", ".pdb");
				resolver = new SymbolResolver(pdbPath);
			} catch(Win32Exception err) when(err.ErrorCode == IncompleteReadException.ErrorNumber) {
				throw new IncompleteReadException(err);
			}

			var g_engineSymb = resolver.findGlobal("g_engine");
			g_engineAddr = g_engineSymb.relativeVirtualAddress + (uint)process.MainModule.BaseAddress;
		}

		public bool Connected {
			get => process != null && !process.HasExited;
		}

		public BaseEngineAccessor GetEngine() {
			var enginePtrVal = memoryReader.ReadUInt32At(g_engineAddr);

			if(enginePtrVal == 0) return null;

			string mangledName = rttiReader.GetMangledClassNameFromObjPtr(enginePtrVal);


			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVScummEngine@Scumm@@")) {
				return new ScummEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVSkyEngine@Sky@@")) {
				return new SkyEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVToonEngine@Toon@@")) {
				return new ToonEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVQueenEngine@Queen@@")) {
				return new QueenEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVPinkEngine@Pink@@")) {
				return new PinkEngineAccessor(this, enginePtrVal);
			}

			return new UnknownEngineAccessor(this, enginePtrVal);
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
