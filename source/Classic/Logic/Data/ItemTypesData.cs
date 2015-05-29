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
using Burntime.Framework.States;
using Burntime.Framework;
using Burntime.Classic.Logic;
using Burntime.Platform.IO;
using Burntime.Platform.Resource;
using Burntime.Data.BurnGfx.Save;

namespace Burntime.Classic.Logic.Data
{
    public class ItemTypesData : DataObject
    {
        public sealed class DataProcessor : IDataProcessor
        {
            public DataObject Process(ResourceID id, ResourceManager resourceManager)
            {
                ConfigFile file = new ConfigFile();
                file.Open(id.File);

                return new ItemTypesData(file, resourceManager);
            }

            string[] IDataProcessor.Names
            {
                get { return new string[] { "items" }; }
            }
        }

        List<ItemTypeData> list;
        string[] burnGfxIDs;

        public ItemTypeData[] Items
        {
            get { return list.ToArray(); }
        }

        public string[] BurnGfxIDs
        {
            get { return burnGfxIDs; }
        }

        protected ItemTypesData(ConfigFile file, ResourceManager resourceManager)
        {
            list = new List<ItemTypeData>();

            // create burngfx id to string convertion array
            burnGfxIDs = new string[58];

            // load item types
            ConfigSection[] sections = file.GetAllSections();
            foreach (ConfigSection section in sections)
            {
                ItemTypeData type = new ItemTypeData();

                if (section.ContainsKey("burngfx"))
                {
                    // use burngfx attributes
                    int burngfx = section.GetInt("burngfx");
                    type.Sprite = "gst.raw?" + burngfx;
                    type.Title = "@burn?" + (50 + burngfx);
                    type.Text = "@burn?" + (110 + burngfx);
                    type.TradeValue = Burntime.Data.BurnGfx.ConstValues.GetValue(burngfx) / 4.0f;
                    type.EatValue = Burntime.Data.BurnGfx.ConstValues.GetValue(burngfx) / 4.0f;
                    type.DrinkValue = Burntime.Data.BurnGfx.ConstValues.GetValue(burngfx) / 4.0f;

                    burnGfxIDs[burngfx] = section.Name;

                    // use attributes from file
                    if (section.ContainsKey("image"))
                        type.Sprite = section.Get("image");
                    if (section.ContainsKey("title"))
                        type.Title = section.Get("title");
                    if (section.ContainsKey("text"))
                        type.Text = section.Get("text");
                    if (section.ContainsKey("value"))
                        type.TradeValue = section.GetInt("value") / 4.0f;
                    if (section.ContainsKey("value"))
                        type.EatValue = section.GetInt("value") / 4.0f;
                    if (section.ContainsKey("value"))
                        type.DrinkValue = section.GetInt("value") / 4.0f;
                }
                else
                {
                    // use attributes from file
                    type.Sprite = section.Get("image");
                    type.Title = section.Get("title");
                    type.Text = section.Get("text");
                    type.TradeValue = section.GetInt("value") / 4.0f;
                    type.EatValue = section.GetInt("value") / 4.0f;
                    type.DrinkValue = section.GetInt("value") / 4.0f;
                }

                type.Class = section.GetStrings("class");

                type.FoodValue = section.GetInt("food");
                type.WaterValue = section.GetInt("water");
                type.HealValue = section.GetInt("heal");
                type.DamageValue = section.GetInt("damage");
                type.Protection = section.GetStrings("protection");
                type.Production = "";
                type.Full = section.GetString("full");
                type.Empty = section.GetString("empty");
                type.AmmoValue = section.GetInt("ammo");

                if (type.Protection.Length > 0 || type.DamageValue > 0)
                {
                    type.IsSelectable = true;
                }

                list.Add(type);

                if (section.Name == "")
                    resourceManager.RegisterDataObject("item_dummy", type);
                else
                    resourceManager.RegisterDataObject(section.Name, type);
            }
        }
    }
}
