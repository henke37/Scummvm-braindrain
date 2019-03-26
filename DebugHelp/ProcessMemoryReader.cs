using System;
using System.Collections.Generic;

namespace DebugHelp {
	public abstract class ProcessMemoryReader : IProcessMemoryReader {

		protected Byte[] scratchBuff=new Byte[16];

		public byte[] ReadBytes(uint addr, uint size) {
			var buff = new Byte[size];
			ReadBytes(addr, size, buff);
			return buff;
		}

		public abstract void ReadBytes(uint addr, uint size, byte[] buff);

		public int ReadInt32At(uint addr) {
			ReadBytes(addr, 4, scratchBuff);
			return scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24);
		}

		public uint ReadUInt32At(uint addr) {
			ReadBytes(addr, 4, scratchBuff);
			return (uint)(scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24));
		}
	}
}
