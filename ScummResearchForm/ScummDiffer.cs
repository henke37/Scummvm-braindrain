using System;
using System.Collections.Generic;
using DrainLib.Engines;

namespace ScummResearchForm {
	internal class ScummDiffer {

		ScummState prevState;

		public Dictionary<int, bool> IgnoredVars;
		public Dictionary<int, bool> IgnoredBitVars;

		public event Action<string, object, object> DifferenceFound;

		public ScummDiffer() {
			IgnoredVars = new Dictionary<int, bool>();
			IgnoredBitVars = new Dictionary<int, bool>();
		}

		internal void diff(ScummState state) {
			if(prevState == null) prevState = state;

			diffInternal(state);

			prevState = state;
		}

		private void diffInternal(ScummState state) {
			for(var varIndex=0;varIndex<state.ScummVars.Length;++varIndex) {
				if(IgnoredVars.ContainsKey(varIndex)) continue;
				var newVal = state.ScummVars[varIndex];
				var oldVal = prevState.ScummVars[varIndex];
				if(newVal == oldVal) continue;

				DifferenceFound?.Invoke($"scummvar{varIndex}", oldVal, newVal);
			}

			for(var bitVarByte=0; bitVarByte < state.bitVarData.Length;bitVarByte++) {
				var newData = state.bitVarData[bitVarByte];
				var oldData = prevState.bitVarData[bitVarByte];
				if(newData == oldData) continue;

				//bit juggling to extract the actual changed bitvars
				for(var bitIndex=0;bitIndex<8;++bitIndex) {
					int varIndex = bitVarByte * 8 + bitIndex;
					if(IgnoredBitVars.ContainsKey(varIndex)) continue;
					bool newBit = (newData & (1 << bitIndex)) != 0;
					bool oldBit = (oldData & (1 << bitIndex)) != 0;
					if(newBit == oldBit) continue;

					DifferenceFound?.Invoke($"bitvar{varIndex}", oldBit, newBit);
				}
			}
		}
	}
}