using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace DebugHelp {
	public abstract class ProcessMemoryReader {

		public byte[] ReadBytes(uint addr, uint size) {
			var buff = new Byte[size];
			ReadBytes(addr, size, buff);
			return buff;
		}

		[SecuritySafeCritical]
		public abstract void ReadBytes(uint addr, uint size, byte[] buff);

		[SecurityCritical]
		public virtual unsafe void ReadBytes(uint addr, uint size, void* buff) {
			Byte[] buffArr = new byte[size];
			ReadBytes(addr, size, buffArr);
			Marshal.Copy(buffArr, 0, (IntPtr)buff, (int)size);
		}

		public Byte ReadByte(uint addr) {
			ReadBytes(addr, 1, scratchBuff);
			return scratchBuff[0];
		}

		[SecuritySafeCritical]
		public unsafe void ReadStruct<T>(uint addr, ref T buff) where T : unmanaged {
			fixed (void* buffP = &buff) {
				ReadBytes(addr, (uint)sizeof(T), buffP);
			}
		}

		public string ReadNullTermString(uint addr) {
			List<Byte> buff=new List<byte>();
			for(; ; addr++) {
				Byte b = ReadByte(addr);
				if(b == 0) break;
				buff.Add(b);
			}

			return new string(Encoding.UTF8.GetChars(buff.ToArray()));
		}

		public int ReadInt32(uint addr) {
			ReadBytes(addr, 4, scratchBuff);
			return scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24);
		}

		public uint ReadUInt32(uint addr) {
			ReadBytes(addr, 4, scratchBuff);
			return (uint)(scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24));
		}

		public short ReadInt16(uint addr) {
			ReadBytes(addr, 2, scratchBuff);
			return (short)(scratchBuff[0] | (scratchBuff[1] << 8));
		}

		public ushort ReadUInt16(uint addr) {
			ReadBytes(addr, 2, scratchBuff);
			return (ushort)(scratchBuff[0] | (scratchBuff[1] << 8));
		}

		public int[] ReadInt32Array(uint addr, uint count) {
			Int32[] arr = new int[count];
			uint byteC = count * 4;
			Byte[] buff = GetReadBuff(byteC);

			ReadBytes(addr, byteC, buff);

			for(uint i = 0; i < count; ++i) {
				arr[i]=buff[0+i*4] | (buff[1 + i * 4] << 8) | (buff[2 + i * 4] << 16) | (buff[3 + i * 4] << 24);
			}

			return arr;
		}

		public uint[] ReadUInt32Array(uint addr, uint count) {
			UInt32[] arr = new uint[count];
			uint byteC = count * 4;
			Byte[] buff = GetReadBuff(byteC);

			ReadBytes(addr, byteC, buff);

			for(uint i = 0; i < count; ++i) {
				arr[i] = (uint)(buff[0 + i * 4] | (buff[1 + i * 4] << 8) | (buff[2 + i * 4] << 16) | (buff[3 + i * 4] << 24));
			}

			return arr;
		}


		protected Byte[] scratchBuff = new Byte[16];

		protected byte[] GetReadBuff(uint count) {
			return count <= scratchBuff.Length ? scratchBuff : new byte[count];
		}
	}
}
