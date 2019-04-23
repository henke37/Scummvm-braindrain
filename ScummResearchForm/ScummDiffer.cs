﻿using System;
using System.Collections.Generic;
using DrainLib.Engines;

namespace ScummResearchForm {
	internal class ScummDiffer {

		ScummState prevState;

		Dictionary<int, bool> ignoredVars;

		public ScummDiffer() {
			ignoredVars = new Dictionary<int, bool>();
		}

		internal void diff(ScummState state) {
			if(prevState == null) prevState = state;

			diffInternal(state);

			prevState = state;
		}

		private void diffInternal(ScummState state) {
			for(var varIndex=0;varIndex<state.ScummVars.Length;++varIndex) {
				if(ignoredVars.ContainsKey(varIndex)) continue;
				var newVal = state.ScummVars[varIndex];
				var oldVal = prevState.ScummVars[varIndex];
				if(newVal == oldVal) continue;
			}

			for(var bitVarByte=0; bitVarByte < state.bitVarData.Length;bitVarByte++) {
				var newData = state.bitVarData[bitVarByte];
				var oldData = prevState.bitVarData[bitVarByte];
				if(newData == oldData) continue;

				//bit juggling to extract the actual changed bitvars
				for(var bitIndex=0;bitIndex<7;++bitIndex) {
					bool newBit = (newData & 1 << bitIndex) != 0;
					bool oldBit = (oldData & 1 << bitIndex) != 0;
					if(newBit == oldBit) continue;
				}
			}
		}
	}
}