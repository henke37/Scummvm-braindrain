using Dia2Lib;
using System;
using System.Collections.Generic;

namespace DebugHelp {
	public class SymbolResolver {
		private DiaSource source;
		private IDiaSession session;

		public SymbolResolver(string pdbPath) {
			source=new DiaSource();
			source.loadDataFromPdb(pdbPath);
			source.openSession(out session);
		}

		public IDiaSymbol findGlobal(string symbolName) {
			session.findChildren(session.globalScope, SymTagEnum.SymTagData, symbolName, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}
	}
}
