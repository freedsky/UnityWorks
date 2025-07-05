using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodDecayServer
{
    public class MMO_MemoryStream : MemoryStream
    {
        //构造函数
        public MMO_MemoryStream() { }

        public MMO_MemoryStream(byte[] buffer) : base(buffer)
        { }

        #region Short
        //从流中读取一个short
        public short ReadShort()
        {
            byte[] buffer = new byte[2];
            base.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }
        //把一个short写入流中
        public void WriteShort(short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region UShort
        //从流中读取一个ushort
        public ushort ReadUShort()
        {
            byte[] buffer = new byte[2];
            base.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }
        //把一个ushort写入流中
        public void WriteUShort(ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Int
        //从流中读取一个int
        public int ReadInt()
        {
            byte[] buffer = new byte[4];
            base.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
        //把一个int写入流中
        public void WriteInt(int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region UInt
        //从流中读取一个uint
        public uint ReadUInt()
        {
            byte[] buffer = new byte[4];
            base.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }
        //把一个uint写入流中
        public void WriteUint(uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Long
        //从流中读取一个long
        public long ReadLong()
        {
            byte[] buffer = new byte[8];
            base.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }
        //把一个long写入流中
        public void WriteLong(long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region ULong
        //从流中读取一个ulong
        public ulong ReadULong()
        {
            byte[] buffer = new byte[8];
            base.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }
        //把一个ulong写入流中
        public void WriteULong(ulong value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Float
        //从流中读取一个float
        public float ReadFloat()
        {
            byte[] buffer = new byte[4];
            base.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }
        //把一个float写入流中
        public void WriteFloat(float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Double
        //从流中读取一个double
        public double ReadDouble()
        {
            byte[] buffer = new byte[8];
            base.Read(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }
        //把一个double写入流中
        public void WriteDouble(double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region Bool
        //从流中读取一个bool
        public bool ReadBool()
        {
            return base.ReadByte() == 1;
        }
        //把一个bool写入流中
        public void WriteBool(bool value)
        {
            base.WriteByte((byte)(value == true ? 1 : 0));
        }
        #endregion

        #region String
        //从流中读取一个string
        public string ReadUTF8String()
        {
            ushort len = this.ReadUShort();
            byte[] buffer = new byte[len];
            base.Read(buffer, 0, len);
            return Encoding.UTF8.GetString(buffer);
        }
        //把一个string写入流中
        public void WriteString(string value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            if (buffer.Length > 65535)
            {
                throw new InvalidCastException("字符长度超出范围");
            }
            WriteUShort((ushort)buffer.Length);
            base.Write(buffer, 0, buffer.Length);
        }
        #endregion
    }
}
