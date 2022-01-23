
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
using System.IO;

namespace DrainLib {
	public class ScummVMConnector {
		private const string executableName = "scummvm.exe";

		private NativeProcess? process;
		internal LiveProcessMemoryAccessor? rawMemoryReader;
		internal CachedProcessMemoryAccessor? cachedMemoryReader;
		internal ReadOnlyCachedProcessMemoryAccessor? readOnlyCachedMemoryReader;
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

				resolver = new SymbolResolver();

				var modules = process.GetModules().ToList();
				ModuleEntry mainModule = modules.First(m => m.Name == executableName);

				foreach(var module in modules) {
					string pdbPath = PDBForModule(module);
					if(!File.Exists(pdbPath)) continue;
					resolver.AddPdb(pdbPath, module.BaseAddress);
				}

				rawMemoryReader = new LiveProcessMemoryAccessor(process);
				cachedMemoryReader = new CachedProcessMemoryAccessor(rawMemoryReader);
				readOnlyCachedMemoryReader = new ReadOnlyCachedProcessMemoryAccessor(cachedMemoryReader, process);
				rttiReader = new RTTIReader(cachedMemoryReader);

				var g_engineSymb = resolver.FindGlobal("g_engine");
				g_engineAddr = mainModule.BaseAddress + (int)g_engineSymb.relativeVirtualAddress;
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteReadException.ErrorNumber) {
				process = null;
				throw new IncompleteReadException(err);
			}
		}

		private string PDBForModule(ModuleEntry module) {
			int lastDot=module.Path.LastIndexOf('.');
			return module.Path.Substring(0, lastDot) + ".pdb";
		}

		public bool Connected {
			get => process != null && !process.HasExited;
		}

		public BaseEngineAccessor? GetEngine() {
			if(!Connected) return null;
			IntPtr enginePtrVal = cachedMemoryReader!.ReadIntPtr(g_engineAddr);

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
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVAGSEngine@AGS@@")) {
				return new AGSEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVPrivateEngine@Private@@")) {
				return new PrivateEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVTinselEngine@Tinsel@@")) {
				return new TinselEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVDirectorEngine@Director@@")) {
				return new DirectorEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVHypnoEngine@Hypno@@")) {
				return new HypnoEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVDraciEngine@Draci@@")) {
				return new DraciEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVNancyEngine@Nancy@@")) {
				return new NancyEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVAvalancheEngine@Avalanche@@")) {
				return new AvalancheEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVMadeEngine@Made@@")) {
				return new MadeEngineAccessor(this, enginePtrVal);
			}
			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVAGOSEngine@AGOS@@")) {
				return new AGOSEngineAccessor(this, enginePtrVal);
			}

			if(rttiReader.HasBaseClass(enginePtrVal, ".?AVTestbedEngine@Testbed@@")) {
				return new TestBedEngineAccessor(this, enginePtrVal);
			}


#pragma warning restore CS8602 // Dereference of a possibly null reference.
			return new UnknownEngineAccessor(this, enginePtrVal);
		}

		public void ClearCache() {
			cachedMemoryReader!.ClearCache();
		}
	}
}
