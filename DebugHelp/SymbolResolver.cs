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

		public IDiaSymbol FindGlobal(string symbolName) {
			session.findChildren(session.globalScope, SymTagEnum.SymTagData, symbolName, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public IDiaSymbol FindClass(string className) {
			session.findChildren(session.globalScope, SymTagEnum.SymTagUDT, className, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public IDiaSymbol FindField(IDiaSymbol engSymb, string fieldName) {
			session.findChildren(engSymb, SymTagEnum.SymTagData, fieldName, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public uint FieldOffset(IDiaSymbol engSymb, string fieldName) {
			IDiaSymbol field = FindField(engSymb, fieldName);
			return (uint)field.offset;
		}

		public uint FieldSize(IDiaSymbol engSymb, string fieldName) {
			IDiaSymbol field = FindField(engSymb, fieldName);
			return (uint)field.type.length;
		}
	}
}
