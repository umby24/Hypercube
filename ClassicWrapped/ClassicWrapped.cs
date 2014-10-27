using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace ClassicWrapped
{
    public class ClassicWrapped
    {
        public NetworkStream Stream;
        private byte[] _buffer;

        public byte ReadByte() {
            return (byte)Stream.ReadByte();
        }

        public sbyte ReadSByte() {
            return (sbyte)Stream.ReadByte();
        }

        public short ReadShort() {
            var shortBytes = ReadBytes(2);
            Array.Reverse(shortBytes);

            return BitConverter.ToInt16(shortBytes, 0);
        }

        public int ReadInt() { // -- Used by Classic Protocol Extension
            var intBytes = ReadBytes(4);
            Array.Reverse(intBytes);

            return BitConverter.ToInt32(intBytes, 0);
        }

        public string ReadString() {
            return Encoding.ASCII.GetString(ReadBytes(64)).Trim(' ');
        }

        public byte[] ReadByteArray() {
            return ReadBytes(1024);
        }

        // -- Writing functions

        public void WriteByte(byte send) {
            WriteBytes(new[] { send });
        }

        public void WriteSByte(sbyte send) {
            WriteBytes(new [] { (byte)send });
        }

        public void WriteShort(short send) {
            var shortBytes = BitConverter.GetBytes(send);
            Array.Reverse(shortBytes);

            WriteBytes(shortBytes);
        }

        public void WriteInt(int send) { // -- Used by Classic Protocol Extension
            var intBytes = BitConverter.GetBytes(send);
            Array.Reverse(intBytes);

            WriteBytes(intBytes);
        }

        public void WriteString(string send) {
            send = send.PadRight(64, ' ');
            WriteBytes(Encoding.ASCII.GetBytes(send));
        }

        public void WriteByteArray(byte[] send) {
            WriteBytes(send);
        }

        // -- Helping functions

        private byte[] ReadBytes(int size) {
            var myBytes = new byte[size];
            var offset = 0;

            while (true) {
                var bytesRead = Stream.Read(myBytes, offset, size);

                if (bytesRead == size)
                    return myBytes;

                size -= bytesRead;
                offset = (bytesRead - 1);
            }
        }

        private void WriteBytes(byte[] bytes) { // -- Writes bytes to the pending send buffer
            if (_buffer == null) {
                _buffer = bytes;
                return;
            }

            var tempLength = _buffer.Length + bytes.Length;
            var tempBuff = new byte[tempLength];

            Buffer.BlockCopy(_buffer, 0, tempBuff, 0, _buffer.Length);
            Buffer.BlockCopy(bytes, 0, tempBuff, _buffer.Length, bytes.Length);

            _buffer = tempBuff;
        }

        public void Purge() { // -- Writes the send buffer to the client
            try {
                Stream.Write(_buffer, 0, _buffer.Length);
                _buffer = null;
            }
            catch (Exception) {
                Stream.Close();
            }
        }
    }
}
