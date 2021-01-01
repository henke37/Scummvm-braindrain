
using Henke37.DebugHelp;
using DrainLib.Engines;
using System;
using System.Linq;
using System.Runtime.Serialization;
using Henke37.DebugHelp.RTTI.MSVC;
using Henke37.DebugHelp.PdbAccess;
using System.ComponentModel;
using Henke37.Win32.AccessRights;
using Henke37.Win32.Processes;
using Henke37.Win32.Snapshots;
using Henke37.Win32.Memory;

namespace DrainLib {
	public class ScummVMConnector {
		private const string executableName = "scummvm.exe";

		private NativeProcess? process;
		internal ProcessMemoryAccessor? memoryReader;
		internal RTTIReader? rttiReader;
		internal SymbolResolver? resolver;

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

				resolver = new SymbolResolver();
				resolver.AddPdb(pdbPath, mainModule.BaseAddress);

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

		public BaseEngineAccessor? GetEngine() {
			if(!Connected) return null;
			IntPtr enginePtrVal = memoryReader!.ReadIntPtr(g_engineAddr);

			if(enginePtrVal == IntPtr.Zero) return null;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
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
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVSupernovaEngine@Supernova@@")) {
				return new SupernovaEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVTuckerEngine@Tucker@@")) {
				return new TuckerEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVTeenAgentEngine@TeenAgent@@")) {
				return new TeenAgentEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVHDBGame@HDB@@")) {
				return new HyperspaceDeliveryBoyEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVSherlockEngine@Sherlock@@")) {
				return new SherlockEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVSciEngine@Sci@@")) {
				return new SciEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVPlumbersGame@Plumbers@@")) {
				return new PlumbersEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVHopkinsEngine@Hopkins@@")) {
				return new HopkinsEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVSludgeEngine@Sludge@@")) {
				return new SludgeEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVSagaEngine@Saga@@")) {
				return new SagaEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVVoyeurEngine@Voyeur@@")) {
				return new VoyeurEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVNeverhoodEngine@Neverhood@@")) {
				return new NeverhoodEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVHadeschEngine@Hadesch@@")) {
				return new HadesChEngineAccessor(this, enginePtrVal);
			}

			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVTestbedEngine@Testbed@@")) {
				return new TestBedEngineAccessor(this, enginePtrVal);
			}


#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
