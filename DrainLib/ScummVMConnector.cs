
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

				foreach(var module in process.GetModules()) {
					string pdbPath = PDBForModule(module);
					if(!File.Exists(pdbPath)) continue;
					resolver.AddPdb(pdbPath, module.BaseAddress);
				}

				rawMemoryReader = new LiveProcessMemoryAccessor(process);
				cachedMemoryReader = new CachedProcessMemoryAccessor(rawMemoryReader);
				readOnlyCachedMemoryReader = new ReadOnlyCachedProcessMemoryAccessor(cachedMemoryReader, process);
				rttiReader = new RTTIReader(cachedMemoryReader);

				var g_engineSymb = resolver.FindGlobal("g_engine");
				g_engineAddr = (IntPtr)g_engineSymb.virtualAddress;
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
			IntPtr engineAddr;

			if(enginePtrVal == IntPtr.Zero) return null;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVScummEngine@Scumm@@", out engineAddr)) {
				return new ScummEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVSkyEngine@Sky@@", out engineAddr)) {
				return new SkyEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVToonEngine@Toon@@", out engineAddr)) {
				return new ToonEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVQueenEngine@Queen@@", out engineAddr)) {
				return new QueenEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVPinkEngine@Pink@@", out engineAddr)) {
				return new PinkEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVDrasculaEngine@Drascula@@", out engineAddr)) {
				return new DrasculaEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVBladeRunnerEngine@BladeRunner@@", out engineAddr)) {
				return new BladeRunnerEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVSupernovaEngine@Supernova@@", out engineAddr)) {
				return new SupernovaEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVTuckerEngine@Tucker@@", out engineAddr)) {
				return new TuckerEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVTeenAgentEngine@TeenAgent@@", out engineAddr)) {
				return new TeenAgentEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVHDBGame@HDB@@", out engineAddr)) {
				return new HyperspaceDeliveryBoyEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVSherlockEngine@Sherlock@@", out engineAddr)) {
				return new SherlockEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVSciEngine@Sci@@", out engineAddr)) {
				return new SciEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVPlumbersGame@Plumbers@@", out engineAddr)) {
				return new PlumbersEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVHopkinsEngine@Hopkins@@", out engineAddr)) {
				return new HopkinsEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVSludgeEngine@Sludge@@", out engineAddr)) {
				return new SludgeEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVSagaEngine@Saga@@", out engineAddr)) {
				return new SagaEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVVoyeurEngine@Voyeur@@", out engineAddr)) {
				return new VoyeurEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVNeverhoodEngine@Neverhood@@", out engineAddr)) {
				return new NeverhoodEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVHadeschEngine@Hadesch@@", out engineAddr)) {
				return new HadesChEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVAGSEngine@AGS@@", out engineAddr)) {
				return new AGSEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVPrivateEngine@Private@@", out engineAddr)) {
				return new PrivateEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVTinselEngine@Tinsel@@", out engineAddr)) {
				return new TinselEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVDirectorEngine@Director@@", out engineAddr)) {
				return new DirectorEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVHypnoEngine@Hypno@@", out engineAddr)) {
				return new HypnoEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVDraciEngine@Draci@@", out engineAddr)) {
				return new DraciEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVNancyEngine@Nancy@@", out engineAddr)) {
				return new NancyEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVAvalancheEngine@Avalanche@@", out engineAddr)) {
				return new AvalancheEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVMadeEngine@Made@@", out engineAddr)) {
				return new MadeEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVAGOSEngine@AGOS@@", out engineAddr)) {
				return new AGOSEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVMTropolisEngine@MTropolis@@", out engineAddr)) {
				return new MTropolisEngineAccessor(this, engineAddr);
			}
			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVGrimEngine@Grim@@", out engineAddr)) {
				return new GrimEngineAccessor(this, engineAddr);
			}

			if(rttiReader.TryDynamicCast(enginePtrVal, ".?AVTestbedEngine@Testbed@@", out engineAddr)) {
				return new TestBedEngineAccessor(this, engineAddr);
			}


#pragma warning restore CS8602 // Dereference of a possibly null reference.
			return new UnknownEngineAccessor(this, enginePtrVal);
		}

		public void ClearCache() {
			cachedMemoryReader!.ClearCache();
		}
	}
}
