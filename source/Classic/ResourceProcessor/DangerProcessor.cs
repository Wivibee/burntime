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

using Burntime.Platform.Resource;
using Burntime.Classic.Logic.Interaction;

namespace Burntime.Classic.ResourceProcessor
{
    class DangerProcessor : IDataProcessor
    {
        public DataObject Process(ResourceID id, ResourceManager resourceManager)
        {
            Danger danger;

            int value = 0;
            if (!int.TryParse(id.Custom, out value))
                Burntime.Platform.Log.Warning("DangerProcessor: custom parameter is not an integer.");

            switch (id.File)
            {
                case "radiation":
                    danger = new Danger(id.File, value, resourceManager.GetString("burn?408"), resourceManager.GetImage("inf.ani?3"));
                    break;
                case "gas":
                    danger = new Danger(id.File, value, resourceManager.GetString("burn?413"), resourceManager.GetImage("inf.ani?7"));
                    break;
                default:
                    throw new Burntime.Framework.BurntimeLogicException();
            }

            return danger;
        }

        string[] IDataProcessor.Names
        {
            get { return new string[] { "danger" }; }
        }
    }
}
