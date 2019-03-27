using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DebugHelp {
	public abstract class ProcessMemoryReader {

		protected Byte[] scratchBuff = new Byte[16];

		public byte[] ReadBytes(uint addr, uint size) {
			var buff = new Byte[size];
			ReadBytes(addr, size, buff);
			return buff;
		}

		public abstract void ReadBytes(uint addr, uint size, byte[] buff);

		public virtual unsafe void ReadBytes(uint addr, uint size, void* buff) {
			throw new NotImplementedException();
		}

		public unsafe void ReadStruct<T>(uint addr, ref T buff) where T : struct {
			GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
			void* buffP = (void*)handle.AddrOfPinnedObject();
			ReadBytes(addr, (uint)Marshal.SizeOf(typeof(T)), buffP);
			handle.Free();
		}

		public string ReadNullTermString(uint addr) {
			throw new NotImplementedException();
		}

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
