using System;
using System.Text;

namespace Unify.Common
{
    public class ByteBuffer
    {
        private int _head;
        private byte[] _data;

        public ByteBuffer(int size)
        {
            _head = 0;
            _data = new byte[size];
        }
        public ByteBuffer(byte[] data)
        {
            _head = 0;
            _data = data;
        }

        #region WriteMethods

        public void WriteInt16(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteInt32(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteInt64(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteUInt32(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteUInt16(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteUInt64(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteByte(byte value)
        {
            var bytes = new[] { value };
            MergeArrays(bytes);
        }
        public void WriteByte(int value)
        {
            var bytes = new[] { (byte)value };
            MergeArrays(bytes);
        }
        public void WriteChar(char value)
        {
            var bytes = new[] { (byte)value };
            MergeArrays(bytes);
        }
        public void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }
        public void WriteBytes(byte[] value)
        {
            var length = value.Length;
            MergeArrays(BitConverter.GetBytes(length));
            MergeArrays(value);
        }
        public void WriteBlock(byte[] value)
        {
            MergeArrays(value);
        }

        public void WriteString(string value)
        {
            _data[_head] = (byte) value.Length;
            _head++;
            MergeArrays(Encoding.ASCII.GetBytes(value));
        }
        public void WriteBool(bool value)
        {
            var bytes = BitConverter.GetBytes(value);
            MergeArrays(bytes);
        }

        #endregion

        #region ReadMethods

        public short ReadInt16()
        {
            _head += 2;
            return BitConverter.ToInt16(_data, _head - 2);
        }
        public int ReadInt32()
        {
            _head += 4;
            return BitConverter.ToInt32(_data, _head - 4);
        }
        public long ReadInt64()
        {
            _head += 8;
            return BitConverter.ToInt64(_data, _head - 8);
        }
        public ushort ReadUInt16()
        {
            _head += 2;
            return BitConverter.ToUInt16(_data, _head - 2);
        }
        public uint ReadUInt32()
        {
            _head += 4;
            return BitConverter.ToUInt32(_data, _head - 4);
        }
        public ulong ReadUInt64()
        {
            _head += 8;
            return BitConverter.ToUInt64(_data, _head - 8);
        }
        public bool ReadBool()
        {
            _head += 1;
            return BitConverter.ToBoolean(_data, _head - 1);
        }
        public byte ReadByte()
        {
            _head += 1;
            return _data[_head - 1];
        }
        public byte[] ReadBytes()
        {
            var length = ReadInt32();
            var bytes = new byte[length];
            Buffer.BlockCopy(_data, _head, bytes, 0, length);
            _head += length;
            return bytes;
        }
        public byte[] ReadBlock(int count)
        {
            var bytes = new byte[count];
            Buffer.BlockCopy(_data, _head, bytes, 0, count);
            _head += count;
            return bytes;
        }
        public char ReadChar()
        {
            _head += 1;
            return (char)_data[_head - 1];
        }
        public double ReadDouble()
        {
            _head += 8;
            return BitConverter.ToDouble(_data, _head - 8);
        }
        public float ReadFloat()
        {
            _head += 4;
            return BitConverter.ToSingle(_data, _head - 4);
        }
        public string ReadString()
        {
            var length = ReadByte();
            var bytes = new byte[length];
            Buffer.BlockCopy(_data, _head, bytes, 0, length);
            _head += length;
            return Encoding.ASCII.GetString(bytes);
        }
        #endregion

        #region InsertMethods
        public void InsertByte(byte b)
        {
            var data = new byte[_data.Length + 1];
            Buffer.BlockCopy(_data, 0, data, 1, _data.Length);
            data[0] = b;
            _head++;
            _data = data;
        }
        public void InsertSize()
        {
            var data = new byte[_data.Length + 2];
            var size = _head;
            Buffer.BlockCopy(_data, 0, data, 2, _data.Length);
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)size), 0, data, 0, 2);
            _head += 2;
            _data = data;
        }
        #endregion

        #region PublicMethods
        public void Skip(int i = 1)
        {
            _head += i;
        }
        public bool IsDoneReading()
        {
            return _head == _data.Length;
        }
        public int GetHead()
        {
            return _head;
        }

        public void ResetHead()
        {
            _head = 0;
        }
        public byte[] GetData()
        {
            return _data;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var b in _data)
            {
                sb.Append(b + "|");
            }
            return sb.ToString();
        }

        #endregion

        #region PrivateMethods
        private void MergeArrays(byte[] bytes)
        {
            Array.Copy(bytes, 0, _data, _head, bytes.Length);
            _head += bytes.Length;
        }

        #endregion


    }
}