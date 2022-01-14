using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace PacketClass
{
    public class Packet : IDisposable
    {
        public Packet() {}
        public Packet(int id)
        {
            this.SetId(id);
        }
        public Packet(byte[] data)
        {
            this.buffer = (this.readableBuffer = data).ToList();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.buffer = null;
                this.readableBuffer = null;
                this.readPos = 0;
            }
            this.disposed = true;
        }

        public void SetId(int id)
        {
            if (this.id == null)
            {
                this.buffer.InsertRange(0,BitConverter.GetBytes(id));
            }
            else
            {
                byte[] idBytes = BitConverter.GetBytes(id);
                this.buffer[0] = idBytes[0];
                this.buffer[1] = idBytes[1];
                this.buffer[2] = idBytes[2];
                this.buffer[3] = idBytes[3];
            }
            this.id = id;
        }
        public int GetId()
        {
            if (this.id == null)
            {
                this.id = BitConverter.ToInt32(this.buffer.GetRange(0,4).ToArray());
            }
            return (int)this.id;
        }

        public void InitExternal()
        {
            this.source = Source.ExternalClient;
            this.GetId();
            this.readableBuffer = this.buffer.GetRange(4,this.buffer.Count-4).ToArray();
        }

        public void InitLocal(int id)
        {
            this.source = Source.LocalServer;
            this.SetId(id);
        }

        private bool ReadBool(bool moveReadPos)
        {
            bool result = BitConverter.ToBoolean(this.readableBuffer,this.readPos);
            if (moveReadPos) this.readPos++;
            return result;
        }
        private byte ReadByte(bool moveReadPos)
        {
            byte result = this.readableBuffer[this.readPos];
            if (moveReadPos) this.readPos++;
            return result;
        }
        private byte[] ReadBytes(bool moveReadPos, int len)
        {
            byte[] result = this.readableBuffer[this.readPos..len];
            if (moveReadPos) this.readPos += len;
            return result;
        }
        private float ReadFloat(bool moveReadPos)
        {
            float result = BitConverter.ToSingle(this.readableBuffer,this.readPos);
            if (moveReadPos) this.readPos += 4;
            return result;
        }
        private int ReadInt(bool moveReadPos)
        {
            int result = BitConverter.ToInt32(this.readableBuffer,this.readPos);
            if (moveReadPos) this.readPos += 4;
            return result;
        }
        private long ReadLong(bool moveReadPos)
        {
            long result = BitConverter.ToInt64(this.readableBuffer,this.readPos);
            if (moveReadPos) this.readPos += 8;
            return result;
        }
        private short ReadShort(bool moveReadPos)
        {
            short result = BitConverter.ToInt16(this.readableBuffer,this.readPos);
            if (moveReadPos) this.readPos += 2;
            return result;
        }
        private string ReadString(bool moveReadPos)
        {
            int num = ReadInt(moveReadPos);
            string result = Encoding.ASCII.GetString(this.readableBuffer,this.readPos,num);
            if (moveReadPos)
            {
                this.readPos += num;
            }
            return result;
        }
        private Vector2 ReadVector2(bool moveReadPos)
        {
            return new Vector2(ReadFloat(moveReadPos),ReadFloat(moveReadPos));
        }
        private Vector3 ReadVector3(bool moveReadPos)
        {
            return new Vector3(ReadFloat(moveReadPos),ReadFloat(moveReadPos),ReadFloat(moveReadPos));
        }
        private Quaternion ReadQuaternion(bool moveReadPos)
        {
            return new Quaternion(ReadFloat(moveReadPos),ReadFloat(moveReadPos),ReadFloat(moveReadPos),ReadFloat(moveReadPos));
        }

        public dynamic[] Read<T>(bool moveReadPos = true, int ?len = null)
        {
            this.source = Source.ExternalClient;
            bool success = true;
            dynamic _result = null;
            dynamic[] result = new dynamic[2];

            try
            {
                if (this.readableBuffer.Length < this.readPos) success = false;
                Type type = typeof(T);
                _result = 
                    type==Types.@bool ? ReadBool(moveReadPos)
                    : type == Types.@byte ? ReadByte(moveReadPos)
                    : type == Types.@bytes ? ReadBytes(moveReadPos,len!=null?(int)len:1)
                    : type == Types.@float ? ReadFloat(moveReadPos)
                    : type == Types.@int ? ReadInt(moveReadPos)
                    : type == Types.@long ? ReadLong(moveReadPos)
                    : type == Types.@short ? ReadShort(moveReadPos)
                    : type == Types.@string ? ReadString(moveReadPos)
                    : type == Types.@Vector2 ? ReadVector2(moveReadPos)
                    : type == Types.@Vector3 ? ReadVector3(moveReadPos)
                    : type == Types.@Quaternion ? ReadQuaternion(moveReadPos)
                    : null;
            }
            catch
            {
                success = false;
            }

            if (success&&(_result!=null))
            {
                result[0] = true;
                result[1] = _result;
                return result;
            }
            else
            {
                result[0] = false;
                return result;
            }
        }
    
        public void Write(bool value)
        {
            byte[] data = BitConverter.GetBytes(value);
            this.buffer.AddRange(data);
        }
        public void Write(byte value)
        {
            this.buffer.Add(value);
        }
        public void Write(byte[] value)
        {
            this.buffer.AddRange(value);
        }
        public void Write(float value)
        {
            this.buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(int value)
        {
            this.buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(long value)
        {
            this.buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(short value)
        {
            this.buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(string value)
        {
            this.Write(value.Length);
            this.buffer.AddRange(Encoding.ASCII.GetBytes(value));
        }
        public void Write(Vector2 value)
        {
            this.Write(value.x);
            this.Write(value.y);
        }
        public void Write(Vector3 value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
        }
        public void Write(Quaternion value)
        {
            this.Write(value.x);
            this.Write(value.y);
            this.Write(value.z);
            this.Write(value.w);
        }

        public Source source;
        public List<byte> buffer = new List<byte>();
        private byte[] readableBuffer;
        private int readPos = 0;
        private int? id = null;
        private bool disposed = false;

        public enum Source
        {
            LocalServer,
            ExternalClient
        }
        public static class Types
        {
            public static Type @bool = typeof(bool);
            public static Type @byte = typeof(byte);
            public static Type @bytes = typeof(byte[]);
            public static Type @float = typeof(float);
            public static Type @int = typeof(int);
            public static Type @long = typeof(long);
            public static Type @short = typeof(short);
            public static Type @string = typeof(string);
            public static Type @Vector2 = typeof(Vector2);
            public static Type @Vector3 = typeof(Vector3);
            public static Type @Quaternion = typeof(Quaternion);
        }
    }
}