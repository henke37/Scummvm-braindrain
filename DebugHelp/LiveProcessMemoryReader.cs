using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace DebugHelp {
	public sealed class LiveProcessMemoryReader : ProcessMemoryReader {
		private Process process;

		public LiveProcessMemoryReader(Process process) {
			this.process = process;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		public unsafe override void ReadBytes(uint addr, uint size, byte[] buff) {
			int readC;
			fixed (Byte* buffP = buff) {
				bool success=ReadProcessMemory(process.Handle, addr, buffP, size, out readC);
				if(!success) throw new Win32Exception();
			}

			if(readC != size) throw new IncompleteReadException();
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[SuppressUnmanagedCodeSecurity]
		public unsafe override void ReadBytes(uint addr, uint size, void* buff) {
			bool success = ReadProcessMemory(process.Handle, addr, buff, size, out int readC);
			if(!success) throw new Win32Exception();
			if(readC != size) throw new IncompleteReadException();
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		unsafe static extern private bool ReadProcessMemory(
			IntPtr hProcess,
			UInt32 lpBaseAddress,
			[Out] void* lpBuffer,
			uint dwSize,
			out int lpNumberOfBytesRead
		);
	}
}
