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

using Burntime.Platform;
using Burntime.Platform.Resource;
using Burntime.Framework.States;
using Burntime.Data.BurnGfx;

using Burntime.Classic.Logic;

namespace Burntime.Classic
{
    [Serializable]
    public class RoomItemList : StateObject, IItemCollection
    {
        StateLinkList<Item> list;
        int max = Infinite;

        protected override void InitInstance(object[] parameter)
        {
            base.InitInstance(parameter);

            list = Container.CreateLinkList<Item>();
        }

        public bool Add(Item Item)
        {
            if (max != Infinite && list.Count >= max)
                return false;

            list.Add(Item);
            return true;
        }

        public void Remove(Item Item)
        {
            list.Remove(Item);
        }

        public const int Infinite = -1;
        public int MaxCount
        {
            get { return max; }
            set { max = value; }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public Item this[int Index]
        {
            get { return list[Index]; }
            set { list[Index] = value; }
        }

        public bool Contains(Item item)
        {
            return list.Contains(item);
        }

        // IEnumerable interface implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ItemCollectionEnumerator(this);
        }
    }
}
