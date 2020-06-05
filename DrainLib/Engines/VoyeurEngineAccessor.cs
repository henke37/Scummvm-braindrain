using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib.Engines {
	public class VoyeurEngineAccessor : ADBaseEngineAccessor {

		#region Symbol data
		//VoyeurEngine
		private int svoyOffset;
		private int gameHourOffset;
		private int gameMinuteOffset;
		private int iForceDeathOffset;
		private int isAMOffset;

		//svoy
		private int victimMurderedOffset;
		private int victimNumberOffset;
		private int incriminatedVictimNumberOffset;
		private int eventFlagsOffset;
		#endregion

		private IntPtr svoyAddr;

		public VoyeurEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
		}

		private void LoadSemiStaticData() {
			svoyAddr = MemoryReader.ReadIntPtr(EngineAddr + svoyOffset);
			if(svoyAddr == IntPtr.Zero) throw new InconsistentDataException();
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Voyeur::VoyeurEngine");
			svoyOffset = Resolver.FieldOffset(engineCl, "_voy");
			gameHourOffset = Resolver.FieldOffset(engineCl, "_gameHour");
			gameMinuteOffset = Resolver.FieldOffset(engineCl, "_gameMinute");
			isAMOffset = Resolver.FieldOffset(engineCl, "_isAM");
			iForceDeathOffset = Resolver.FieldOffset(engineCl, "_iForceDeath");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");

			var svoyCl = Resolver.FindClass("Voyeur::SVoy");
			victimMurderedOffset = Resolver.FieldOffset(svoyCl,"_victimMurdered");
			victimNumberOffset = Resolver.FieldOffset(svoyCl, "_victimNumber");
			incriminatedVictimNumberOffset = Resolver.FieldOffset(svoyCl, "_incriminatedVictimNumber");
			eventFlagsOffset = Resolver.FieldOffset(svoyCl, "_eventFlags");

			LoadADSymbols(descriptorOffset, true);
		}

		public VoyeurState GetState() {
			var st = new VoyeurState();
			bool isAM = MemoryReader.ReadByte(EngineAddr + isAMOffset)!=0;
			st.GameHour = MemoryReader.ReadInt32(EngineAddr + gameHourOffset) + (isAM?0:12);
			st.GameMinute = MemoryReader.ReadInt32(EngineAddr + gameMinuteOffset);

			st.VictimMurdered = MemoryReader.ReadByte(svoyAddr + victimMurderedOffset) != 0;
			st.VictimNumber = MemoryReader.ReadInt32(svoyAddr + victimNumberOffset);
			st.IncriminatedVictimNumber = MemoryReader.ReadInt32(svoyAddr + incriminatedVictimNumberOffset);
			st.EventFlags = MemoryReader.ReadInt32(svoyAddr + eventFlagsOffset);
			return st;
		}

		public int ForceDeath => MemoryReader.ReadInt32(EngineAddr + iForceDeathOffset);

		public class VoyeurState {
			public bool VictimMurdered;
			public int VictimNumber;
			public int IncriminatedVictimNumber;
			public int EventFlags;

			public int GameHour;
			public int GameMinute;
		}
	}
}
