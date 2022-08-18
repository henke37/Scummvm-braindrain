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

		//Com::SubReadStream
		private int subStreamParentStreamOffset;

		//Com::Rational
		private int comRationalNumOffset;
		private int comRationalDenomOffset;
		#endregion

		protected ProcessMemoryAccessor MemoryReader => Connector.cachedMemoryReader!;
		protected RTTIReader RttiReader => Connector.rttiReader!;
		protected SymbolResolver Resolver => Connector.resolver!;

		protected BaseAccessor(ScummVMConnector connector) {
			if(connector==null) throw new ArgumentNullException(nameof(connector));
			this.Connector = connector;

			LoadBaseSymbols();
		}

		protected void LoadBaseSymbols() {
			var comStringSymb = Resolver.FindClass("Common::String");
			var comBaseStringSymb = Resolver.GetBaseClass(comStringSymb);
			comStringSizeOffset = Resolver.FieldOffset(comBaseStringSymb, "_size");
			comStringStrOffset = Resolver.FieldOffset(comBaseStringSymb, "_str");

			var comFileClSymb = Resolver.FindClass("Common::File");
			comFileNameOffset = Resolver.FieldOffset(comFileClSymb, "_name");

			var subStreamClSymb = Resolver.FindClass("Common::SubReadStream");
			var parentStreamFieldSymb=Resolver.FindField(subStreamClSymb, "_parentStream");
			var parentStreamClSymb = parentStreamFieldSymb.type;
			subStreamParentStreamOffset = parentStreamFieldSymb.offset + Resolver.FieldOffset(parentStreamClSymb, "_pointer");

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
			if(RttiReader.TryDynamicCast(streamAddr, ".?AVFile@Common@@", out var fileAddr)) return ReadFileNameInternal(fileAddr);
			if(RttiReader.TryDynamicCast(streamAddr, ".?AVSubReadStream@Common@@", out var subStreamAddr)) return ReadSubStreamFileName(subStreamAddr);
			return null;
		}

		private string? ReadSubStreamFileName(IntPtr subStreamAddr) {
			var parentStreamAddr = MemoryReader.ReadIntPtr(subStreamAddr + subStreamParentStreamOffset);
			return ReadFileName(parentStreamAddr);
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
