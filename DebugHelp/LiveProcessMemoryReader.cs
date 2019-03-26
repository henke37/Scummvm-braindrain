using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DebugHelp {
	public class LiveProcessMemoryReader : ProcessMemoryReader {
		private Process process;

		public LiveProcessMemoryReader(Process process) {
			this.process = process;
		}

		public override void ReadBytes(uint addr, uint size, byte[] buff) {
			int readC;
			ReadProcessMemory(process.Handle, (IntPtr)addr, buff, size, out readC);
			if(readC != size) throw new Exception("Read failed to read all the data");
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			[Out] byte[] lpBuffer,
			uint dwSize,
			out int lpNumberOfBytesRead
		);
	}
}
