using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace DebugHelp {
	public class LiveProcessMemoryWriter : ProcessMemoryWriter {
		private Process Process;

		public LiveProcessMemoryWriter(Process process) {
			this.Process = process;
		}

		public override unsafe void WriteBytes(byte[] srcBuff, uint dstAddr, uint size) {
			try { 
				fixed (byte* buffP = srcBuff) {
					bool success = WriteProcessMemory(Process.Handle, (IntPtr)dstAddr,buffP, size, out var written);
					if(!success) throw new Win32Exception();
				}
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteWriteException.ErrorNumber) {
				throw new IncompleteWriteException(err);
			}
		}

		[SecurityCritical]
		public override unsafe void WriteBytes(void* srcBuff, uint dstAddr, uint size) {
			try { 
				bool success = WriteProcessMemory(Process.Handle, (IntPtr)dstAddr, (byte*)srcBuff, size, out var written);
				if(!success) throw new Win32Exception();
			} catch(Win32Exception err) when(err.NativeErrorCode == IncompleteWriteException.ErrorNumber) {
				throw new IncompleteWriteException(err);
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static unsafe extern bool WriteProcessMemory(
		  IntPtr hProcess,
		  IntPtr lpBaseAddress,
		  byte* lpBuffer,
		  UInt32 nSize,
		  out IntPtr lpNumberOfBytesWritten
		);
	}
}
