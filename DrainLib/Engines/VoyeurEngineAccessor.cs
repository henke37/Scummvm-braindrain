﻿using System;
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
		private int controlPtrOffset;

		//svoy
		private int victimMurderedOffset;
		private int incriminatedVictimNumberOffset;
		private int eventFlagsOffset;
		private int RTVNumOffset;
		private int RTVLimitOffset;

		//ControlResource
		private int stateOffset;

		//StateResource
		private int victimIndexOffset;
		#endregion

		private IntPtr svoyAddr;
		private IntPtr ctrlAddr;
		private IntPtr stateAddr;
		private IntPtr victimIndexPtrVal;

		public VoyeurEngineAccessor(ScummVMConnector connector, IntPtr engineAddr) : base(connector, engineAddr) {
			LoadSemiStaticData();
		}

		private void LoadSemiStaticData() {
			svoyAddr = MemoryReader.ReadIntPtr(EngineAddr + svoyOffset);
			if(svoyAddr == IntPtr.Zero) throw new InconsistentDataException();

			ctrlAddr = MemoryReader.ReadIntPtr(EngineAddr + controlPtrOffset);
			if(ctrlAddr == IntPtr.Zero) throw new InconsistentDataException();

			stateAddr = MemoryReader.ReadIntPtr(ctrlAddr + stateOffset);
			if(stateAddr == IntPtr.Zero) throw new InconsistentDataException();

			victimIndexPtrVal = MemoryReader.ReadIntPtr(stateAddr + victimIndexOffset);
		}

		internal override void LoadSymbols() {
			var engineCl = Resolver.FindClass("Voyeur::VoyeurEngine");
			svoyOffset = Resolver.FieldOffset(engineCl, "_voy");
			gameHourOffset = Resolver.FieldOffset(engineCl, "_gameHour");
			gameMinuteOffset = Resolver.FieldOffset(engineCl, "_gameMinute");
			iForceDeathOffset = Resolver.FieldOffset(engineCl, "_iForceDeath");
			var descriptorOffset = Resolver.FieldOffset(engineCl, "_gameDescription");
			controlPtrOffset = Resolver.FieldOffset(engineCl, "_controlPtr");

			var svoyCl = Resolver.FindClass("Voyeur::SVoy");
			isAMOffset = Resolver.FieldOffset(svoyCl, "_isAM");
			victimMurderedOffset = Resolver.FieldOffset(svoyCl,"_victimMurdered");
			incriminatedVictimNumberOffset = Resolver.FieldOffset(svoyCl, "_incriminatedVictimNumber");
			eventFlagsOffset = Resolver.FieldOffset(svoyCl, "_eventFlags");
			RTVNumOffset = Resolver.FieldOffset(svoyCl, "_RTVNum");
			RTVLimitOffset = Resolver.FieldOffset(svoyCl, "_RTVLimit");

			var ctrlCl = Resolver.FindClass("Voyeur::ControlResource");
			stateOffset = Resolver.FieldOffset(ctrlCl, "_state");

			var stateCl = Resolver.FindClass("Voyeur::StateResource");
			victimIndexOffset = Resolver.FieldOffset(stateCl, "_victimIndex");

			LoadADSymbols(descriptorOffset, true);
		}

		public VoyeurState GetState() {
			var st = new VoyeurState();
			bool isAM = MemoryReader.ReadByte(svoyAddr + isAMOffset)!=0;
			st.GameHour = MemoryReader.ReadInt32(EngineAddr + gameHourOffset) + (isAM?0:12);
			st.GameMinute = MemoryReader.ReadInt32(EngineAddr + gameMinuteOffset);
			st.VictimNumber = MemoryReader.ReadInt32(victimIndexPtrVal);
			st.VictimMurdered = MemoryReader.ReadByte(svoyAddr + victimMurderedOffset) != 0;
			st.IncriminatedVictimNumber = MemoryReader.ReadInt32(svoyAddr + incriminatedVictimNumberOffset);
			st.EventFlags = MemoryReader.ReadInt32(svoyAddr + eventFlagsOffset);
			st.RTVNum = MemoryReader.ReadInt32(svoyAddr + RTVNumOffset);
			st.RTVLimit = MemoryReader.ReadInt32(svoyAddr + RTVLimitOffset);
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

			internal int RTVNum;
			internal int RTVLimit;

			public float RTVCharge => (float)RTVNum / RTVLimit;
		}
	}
}
