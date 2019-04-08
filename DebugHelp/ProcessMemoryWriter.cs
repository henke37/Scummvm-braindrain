using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace DebugHelp {
	public abstract class ProcessMemoryWriter {

		public byte[] WriteBytes(uint addr, uint size) {
			var buff = new Byte[size];
			WriteBytes(addr, size, buff);
			return buff;
		}

		[SecuritySafeCritical]
		public abstract void WriteBytes(uint addr, uint size, byte[] buff);

		[SecurityCritical]
		public virtual unsafe void WriteBytes(uint addr, uint size, void* buff) {
			Byte[] buffArr = new byte[size];
			Marshal.Copy((IntPtr)buff, buffArr, 0, (int)size);
			WriteBytes(addr, size, buffArr);
		}

		public Byte WriteByte(uint addr) {
			WriteBytes(addr, 1, scratchBuff);
			return scratchBuff[0];
		}


		protected Byte[] scratchBuff = new Byte[16];

		protected byte[] GetReadBuff(uint count) {
			return count <= scratchBuff.Length ? scratchBuff : new byte[count];
		}
	}
}
