using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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

		public Byte ReadByte(uint addr) {
			ReadBytes(addr, 1, scratchBuff);
			return scratchBuff[0];
		}

		public unsafe void ReadStruct<T>(uint addr, ref T buff) where T : struct {
			GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
			void* buffP = (void*)handle.AddrOfPinnedObject();
			ReadBytes(addr, (uint)Marshal.SizeOf(typeof(T)), buffP);
			handle.Free();
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

		public int ReadInt32At(uint addr) {
			ReadBytes(addr, 4, scratchBuff);
			return scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24);
		}

		public uint ReadUInt32At(uint addr) {
			ReadBytes(addr, 4, scratchBuff);
			return (uint)(scratchBuff[0] | (scratchBuff[1] << 8) | (scratchBuff[2] << 16) | (scratchBuff[3] << 24));
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

		private byte[] GetReadBuff(uint count) {
			return scratchBuff.Length <= count ? scratchBuff : new byte[count];
		}
	}
}
