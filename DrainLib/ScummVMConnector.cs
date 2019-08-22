
using Henke37.DebugHelp;
using DrainLib.Engines;
using System;
using System.Linq;
using System.Runtime.Serialization;
using Henke37.DebugHelp.RTTI.MSVC;
using Henke37.DebugHelp.Win32;
using Henke37.DebugHelp.PdbAccess;
using System.ComponentModel;

namespace DrainLib {
	public class ScummVMConnector {
		private const string executableName = "scummvm.exe";

		private NativeProcess process;
		internal ProcessMemoryReader memoryReader;
		internal RTTIReader rttiReader;
		internal SymbolResolver resolver;

		internal IntPtr g_engineAddr;

		public ScummVMConnector() { }

		public void Connect() {
			try {
				using(var snap = new Toolhelp32Snapshot(Toolhelp32SnapshotFlags.Process)) {
					var procEntry = snap.GetProcesses().FirstOrDefault(p => p.Executable == executableName);
					if(procEntry == null) throw new ProcessNotFoundException();
					process = procEntry.Open(ProcessAccessRights.VMOperation | ProcessAccessRights.VMRead | ProcessAccessRights.Synchronize | ProcessAccessRights.QueryInformation);
				}
				ModuleEntry mainModule = process.GetModules().First(m => m.Name == executableName);

				string pdbPath = mainModule.Path.Replace(".exe", ".pdb");
				resolver = new SymbolResolver(pdbPath);

				memoryReader = new LiveProcessMemoryAccessor(process);
				rttiReader = new RTTIReader(memoryReader);

				var g_engineSymb = resolver.FindGlobal("g_engine");
				g_engineAddr = mainModule.BaseAddress + (int)g_engineSymb.relativeVirtualAddress;
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
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVTestbedEngine@Testbed@@")) {
				return new TestBedEngineAccessor(this, enginePtrVal);
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
