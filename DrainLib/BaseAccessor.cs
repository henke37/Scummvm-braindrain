using Henke37.DebugHelp;
using Henke37.DebugHelp.PdbAccess;
using Henke37.DebugHelp.RTTI.MSVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrainLib {
	public abstract class BaseAccessor {
		protected ScummVMConnector Connector;


		#region Symbol data
		//Com::String
		private int comStringSizeOffset;
		private int comStringStrOffset;

		//Com::File
		private int comFileNameOffset;

		//Com::Rational
		private int comRationalNumOffset;
		private int comRationalDenomOffset;
		#endregion

		protected ProcessMemoryReader MemoryReader => Connector.memoryReader!;
		protected RTTIReader RttiReader => Connector.rttiReader!;
		protected SymbolResolver Resolver => Connector.resolver!;

		protected BaseAccessor(ScummVMConnector connector) {
			if(connector==null) throw new ArgumentNullException(nameof(connector));
			this.Connector = connector;

			LoadBaseSymbols();
		}

		protected void LoadBaseSymbols() {
			var comStringSymb = Resolver.FindClass("Common::String");
			comStringSizeOffset = Resolver.FieldOffset(comStringSymb, "_size");
			comStringStrOffset = Resolver.FieldOffset(comStringSymb, "_str");

			var comFileClSymb = Resolver.FindClass("Common::File");
			comFileNameOffset = Resolver.FieldOffset(comFileClSymb, "_name");

			var comRationalClSymb = Resolver.FindClass("Common::Rational");
			comRationalNumOffset = Resolver.FieldOffset(comRationalClSymb, "_num");
			comRationalDenomOffset = Resolver.FieldOffset(comRationalClSymb, "_denom");
		}


		protected string ReadComString(IntPtr addr) {
			uint size = MemoryReader.ReadUInt32(addr + comStringSizeOffset);
			IntPtr strPtr = MemoryReader.ReadIntPtr(addr + comStringStrOffset);

			var strBytes = MemoryReader.ReadBytes(strPtr, size);
			return new string(Encoding.UTF8.GetChars(strBytes));
		}

		protected string? ReadFileName(IntPtr streamAddr) {
			if(!RttiReader.TryDynamicCast(streamAddr, ".?AVFile@Common@@", out var fileAddr)) return null;
			return ReadFileNameInternal(fileAddr);
		}
		protected string ReadFileNameInternal(IntPtr fileAddr) {
			return ReadComString(fileAddr + comFileNameOffset);
		}

		protected Rational ReadRational(IntPtr rationalAddr) {
			Rational r;
			r.Numerator = MemoryReader.ReadInt32(rationalAddr + comRationalNumOffset);
			r.Denominator = MemoryReader.ReadInt32(rationalAddr + comRationalDenomOffset);
			return r;
		}
	}
}
