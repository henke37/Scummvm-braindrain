
using Henke37.DebugHelp;
using Henke37.DebugHelp.RTTI;
using DrainLib.Engines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Henke37.DebugHelp.RTTI.MSVC;
using Henke37.DebugHelp.Win32;
using Henke37.DebugHelp.PdbAccess;

namespace DrainLib {
	public class ScummVMConnector {

		private Process process;
		internal ProcessMemoryReader memoryReader;
		internal RTTIReader rttiReader;
		internal SymbolResolver resolver;

		internal IntPtr g_engineAddr;

		public ScummVMConnector() { }

		public void Connect() {
			var procs = Process.GetProcessesByName("scummvm");
			if(procs.Length == 0) throw new ProcessNotFoundException();

			process = procs[0];
			try {
				string pdbPath = process.MainModule.FileName.Replace(".exe", ".pdb");
				resolver = new SymbolResolver(pdbPath);

				memoryReader = new LiveProcessMemoryAccessor(process);
				rttiReader = new RTTIReader(memoryReader);

				var g_engineSymb = resolver.FindGlobal("g_engine");
				g_engineAddr = process.MainModule.BaseAddress + (int)g_engineSymb.relativeVirtualAddress;
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteReadException.ErrorNumber) {
				process = null;
				throw new IncompleteReadException(err);
			}
		}

		public bool Connected {
			get => process != null && !process.HasExited;
		}

		public BaseEngineAccessor GetEngine() {
			IntPtr enginePtrVal = memoryReader.ReadIntPtr(g_engineAddr);

			if(enginePtrVal == IntPtr.Zero) return null;

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
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVDrasculaEngine@Drascula@@")) {
				return new DrasculaEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVBladeRunnerEngine@BladeRunner@@")) {
				return new BladeRunnerEngineAccessor(this, enginePtrVal);
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
