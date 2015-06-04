﻿
#region The MIT License (MIT) - 2015 Jakob Harder
/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Jakob Harder
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Burntime.Platform.IO
{
    public enum SeekPosition
    {
        Begin,
        Current,
        End
    }

    public class File
    {
        string name;

        // properties
        public virtual bool HasName { get { return false; } }
        public virtual String PackageName { get { return null; } }
        public virtual String FullName { get { return null; } }
        public virtual String Name { get { return name; } }

        public virtual String FullPath { get { return null; } }
        public virtual bool HasFullPath { get { return false; } }

        public bool CanWrite { get { return stream.CanWrite; } }

        public int Length { get { return (int)stream.Length; } }
        public int Position { get { return (int)stream.Position; } }
        public bool IsEOF { get { return stream.Position >= stream.Length - 1; } }

        Encoding encoding;
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        // stream
        protected Stream stream;
        public Stream Stream { get { return stream; } }

        public static implicit operator Stream(File Right)
        {
            return Right.Stream;
        }

        // access

        public void Close()
        {
            stream.Close();
        }

        // constructor
        public File(Stream stream)
        {
            this.stream = stream;
            name = null;
        }

        public File(Stream stream, string name)
        {
            this.stream = stream;
            this.name = name;
        }

        protected File()
        {
        }

        public File GetSubFile(int Start, int End)
        {
            int size = End - Start;
            if (End < Start)
                size = (int)(stream.Length - Start);

            byte[] data = new byte[size];
            Seek(Start, SeekPosition.Begin);
            Read(data, size);
            File file = new File(new MemoryStream(data));
            file.name = name;
            return file;
        }

        // read
        public void Seek(int Pos, SeekPosition origin)
        {
            if (origin == SeekPosition.Begin)
                stream.Seek(Pos, SeekOrigin.Begin);
            else if (origin == SeekPosition.Current)
                stream.Seek(Pos, SeekOrigin.Current);
            else
                stream.Seek(Pos, SeekOrigin.End);
        }

        public byte ReadByte()
        {
            byte[] data = new byte[1];
            int read = stream.Read(data, 0, 1);
            return data[0];
        }

        public ushort ReadUShort()
        {
            byte[] data = new byte[2];
            int read = stream.Read(data, 0, 2);

            return (ushort)(data[0] + (data[1] << 8));
        }

        public int Read(byte[] Data, int Count)
        {
            return Read(Data, 0, Count);
        }

        public int Read(byte[] Data, int Offset, int Count)
        {
            return stream.Read(Data, Offset, Count);
        }

        public byte[] ReadBytes(int Count)
        {
            byte[] bytes = new byte[Count];
            Read(bytes, Count);
            return bytes;
        }

        public byte[] ReadAllBytes()
        {
            byte[] b = new byte[stream.Length];
            stream.Read(b, 0, (int)stream.Length);
            return b;
        }

        // write
        public void Write(byte[] Data, int Count)
        {
            Write(Data, 0, Count);
        }

        public void Write(byte[] Data, int Offset, int Count)
        {
            if (CanWrite)
            {
                stream.Write(Data, Offset, Count);
            }
        }

        public void WriteByte(byte Data)
        {
            if (CanWrite)
                stream.WriteByte(Data);
        }

        public void WriteUShort(ushort Data)
        {
            WriteByte((byte)(Data & 0xff));
            WriteByte((byte)(Data >> 8));
        }

        public void Flush()
        {
            if (CanWrite)
                stream.Flush();
        }

        // text
        StreamWriter textWriter;
        StreamReader textReader;

        public void WriteLine(String Line)
        {
            if (textWriter == null)
            {
                if (encoding != null)
                    textWriter = new StreamWriter(stream, encoding);
                else
                    textWriter = new StreamWriter(stream);
                textWriter.AutoFlush = true;
            }

            textWriter.WriteLine(Line);
        }

        public String ReadLine()
        {
            if (textReader == null)
            {
                if (encoding != null)
                    textReader = new StreamReader(stream, encoding);
                else
                    textReader = new StreamReader(stream);
            }

            return textReader.ReadLine();
        }
    }
}
