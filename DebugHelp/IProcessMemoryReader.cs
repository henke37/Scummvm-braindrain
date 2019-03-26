using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugHelp {
	public interface IProcessMemoryReader {
		Int32 ReadInt32At(uint addr);
		UInt32 ReadUInt32At(uint addr);
		Byte[] ReadBytes(uint addr, uint size);
		void ReadBytes(uint addr, uint size, Byte[] buff);
		unsafe void ReadBytes(uint addr, uint size, void *buff);
	}
}
