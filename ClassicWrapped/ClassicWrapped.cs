using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ClassicWrapped
{
    public class ClassicWrapped
    {
        public NetworkStream _Stream;
        private byte[] buffer;

        public byte ReadByte() {
            return (byte)_Stream.ReadByte();
        }

        public sbyte ReadSByte() {
            return (sbyte)_Stream.ReadByte();
        }

        public short ReadShort() {
            byte[] shortBytes = ReadBytes(2);
            Array.Reverse(shortBytes);

            return BitConverter.ToInt16(shortBytes, 0);
        }

        public int ReadInt() { // -- Used by Classic Protocol Extension
            byte[] intBytes = ReadBytes(4);
            Array.Reverse(intBytes);

            return BitConverter.ToInt32(intBytes, 0);
        }

        public string ReadString() {
            return Encoding.ASCII.GetString(ReadBytes(64)).TrimEnd(' ');
        }

        public byte[] ReadByteArray() {
            return ReadBytes(1024);
        }

        // -- Writing functions

        public void WriteByte(byte Send) {
            if (buffer != null) {
                var tempBuff = new byte[buffer.Length + 1];

                Buffer.BlockCopy(buffer, 0, tempBuff, 0, buffer.Length);
                tempBuff[buffer.Length] = Send;

                buffer = tempBuff;
            } else {
                buffer = new byte[1] { Send };
            }
        }

        public void WriteSByte(sbyte Send) {
            if (buffer != null) {
                var tempBuff = new byte[buffer.Length + 1];

                Buffer.BlockCopy(buffer, 0, tempBuff, 0, buffer.Length);
                tempBuff[buffer.Length] = (byte)Send;

                buffer = tempBuff;
            } else {
                buffer = new byte[1] { (byte)Send };
            }
        }

        public void WriteShort(short Send) {
            byte[] shortBytes = BitConverter.GetBytes(Send);
            Array.Reverse(shortBytes);

            WriteBytes(shortBytes);
        }

        public void WriteInt(int Send) { // -- Used by Classic Protocol Extension
            byte[] intBytes = BitConverter.GetBytes(Send);
            Array.Reverse(intBytes);

            WriteBytes(intBytes);
        }

        public void WriteString(string Send) {
            Send = Send.PadRight(64, ' ');
            WriteBytes(Encoding.ASCII.GetBytes(Send));
        }

        public void WriteByteArray(byte[] Send) {
            WriteBytes(Send);
        }

        // -- Helping functions

        private byte[] ReadBytes(int size) {
            var myBytes = new byte[size];
            int bytesRead;

            bytesRead = _Stream.Read(myBytes, 0, size);

            if (bytesRead == size)
                return myBytes;

            while (true) {
                if (bytesRead != size) {
                    int newSize = size - bytesRead;

                    bytesRead = _Stream.Read(myBytes, bytesRead - 1, newSize);

                    if (bytesRead != newSize)
                        size = newSize;
                    else
                        break;
                }
            }

            return myBytes;
        }

        private void WriteBytes(byte[] bytes) { // -- Writes bytes to the pending send buffer
            if (buffer == null) {
                buffer = bytes;
            } else {
                int tempLength = buffer.Length + bytes.Length;
                byte[] tempBuff = new byte[tempLength];

                Buffer.BlockCopy(buffer, 0, tempBuff, 0, buffer.Length);
                Buffer.BlockCopy(bytes, 0, tempBuff, buffer.Length, bytes.Length);

                buffer = tempBuff;
            }
        }

        public void Purge() { // -- Writes the send buffer to the client
            try {
                _Stream.Write(buffer, 0, buffer.Length);
                buffer = null;
                GC.Collect();
            } catch {

            }
        }
    }
}
