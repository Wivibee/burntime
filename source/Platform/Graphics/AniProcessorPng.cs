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
using System.Runtime.InteropServices;

using Burntime.Platform;
using Burntime.Platform.IO;
using Burntime.Platform.Resource;

namespace Burntime.Platform.Graphics
{
    public class AniProcessorPng : ISpriteProcessor, ISpriteAnimationProcessor, IDataProcessor
    {
        Vector2 size;
        int frameOffset;
        int frameCount;
        string format;
        SpriteProcessorPng png;

        public Vector2 Size
        {
            get { return size; }
        }

        public int FrameCount
        {
            get { return frameCount; }
        }

        public Vector2 FrameSize
        {
            get { return size; }
        }

        public bool SetFrame(int frame)
        {
            png = new SpriteProcessorPng();
            png.Process(String.Format(format, frame + frameOffset));
            size = png.Size;
            return true;
        }

        public void Process(ResourceID ID)
        {
            if (ID.EndIndex == -1)
                frameCount = 1;
            else
                frameCount = ID.EndIndex - ID.Index + 1;

            frameOffset = ID.Index;
            format = ID.File;
            size = new Vector2();
        }

        public DataObject Process(ResourceID ID, ResourceManager ResourceManager)
        {
            return ResourceManager.GetImage(ID);
        }

        public void Render(IntPtr ptr)
        {
            throw new NotSupportedException();
        }

        public void Render(System.IO.Stream s, int stride)
        {
            png.Render(s, stride);
        }

        string[] IDataProcessor.Names
        {
            get { return new string[] { "pngani" }; }
        }
    }
}
