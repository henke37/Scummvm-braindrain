using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace DebugHelp {
	public abstract class ProcessMemoryWriter {

		[SecuritySafeCritical]
		public abstract void WriteBytes(byte[] srcBuff, uint dstAddr, uint size);

		[SecurityCritical]
		public virtual unsafe void WriteBytes(void* srcBuff, uint dstAddr, uint size) {
			Byte[] buffArr = GetWriteBuff(size);
			Marshal.Copy((IntPtr)srcBuff, buffArr, 0, (int)size);
			WriteBytes(buffArr, dstAddr, size);
		}

		public void WriteByte(byte value, uint dstAddr) {
			scratchBuff[0] = value;
			WriteBytes(scratchBuff, dstAddr, 1);
		}

		[SecuritySafeCritical]
		public unsafe void WriteStruct<T>(uint dstAddr, ref T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				WriteBytes(buffP, dstAddr, (uint)sizeof(T));
			}
		}


		protected Byte[] scratchBuff = new Byte[16];

		protected byte[] GetWriteBuff(uint count) {
			return count <= scratchBuff.Length ? scratchBuff : new byte[count];
		}
	}
}
