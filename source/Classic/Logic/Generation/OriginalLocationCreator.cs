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
using Burntime.Data.BurnGfx;
using Burntime.Data.BurnGfx.Save;
using Burntime.Classic.Logic.Interaction;

namespace Burntime.Classic.Logic.Generation
{
    /// <summary>
    /// Creates locations in original order from gamdat/burnmap files.
    /// </summary>
    public class OriginalLocationCreator : IGameObjectCreator
    {
        public void Create(ClassicGame game)
        {
            var gamdat = LogicFactory.GetParameter<Burntime.Data.BurnGfx.Save.SaveGame>("gamdat");
            if (gamdat == null)
                return;

            var container = game.Container;

            for (int i = 1; i <= gamdat.Locations.Length; i++)
            {
                var city = gamdat.Locations[i - 1];

                Location loc = container.Create<Location>();
                loc.Id = game.World.Locations.Count;
                loc.Source.Water = city.WaterSource;
                loc.Source.Reserve = city.Water;
                loc.Source.Capacity = city.WaterCapacity;
                loc.Production = city.Producing == -1 ? null : game.Productions[city.Producing];
                loc.AvailableProducts = (int[])city.Production.Clone();
                if (city.Danger != 0)
                    loc.Danger = Danger.Instance((city.Danger == 3) ? "radiation" : "gas", city.DangerAmount);
                loc.IsCity = city.IsCity;
                loc.EntryPoint = city.EntryPoint;
                loc.Ways = city.Ways;
                loc.WayLengths = city.WayLengths;

                loc.Map = container.Create<Map>(new object[] { "maps/mat_" + i.ToString("D3") + ".burnmap??4" });

                loc.Rooms = container.CreateLinkList<Room>();

                for (int j = 0; j < loc.Map.Entrances.Length; j++)
                {
                    RoomType type = loc.Map.Entrances[j].RoomType;

                    Room room = container.Create<Room>();
                    room.IsWaterSource = type == RoomType.WaterSource;
                    if (type != RoomType.Normal && type != RoomType.Rope && type != RoomType.WaterSource)
                        room.Items.MaxCount = 0;
                    else
                        room.Items.MaxCount = room.IsWaterSource ? 8 : 32;
                    room.EntryCondition.MaxDistanceOnMap = 15;
                    if (loc.Map.Entrances[j].RoomType == RoomType.Rope)
                    {
                        room.EntryCondition.MaxDistanceOnMap = 75;
                        room.EntryCondition.RequiredItem = game.ItemTypes["item_rope"];
                    }
                    room.EntryCondition.RegionOnMap = loc.Map.Entrances[j].Area;
                    room.EntryCondition.HasRegionOnMap = true;
                    room.TitleId = loc.Map.Entrances[j].TitleId;
                    loc.Rooms += room;
                }

                game.World.Locations += loc;
            }

            for (int n = 0; n < game.World.Locations.Count; n++)
            {
                Burntime.Data.BurnGfx.Save.Location info = gamdat.Locations[n];
                for (int k = 0; k < 4; k++)
                {
                    if (info.Neighbors[k] != -1)
                        game.World.Locations[n].Neighbors.Add(game.World.Locations[info.Neighbors[k]]);
                }
            }
        }
    }
}
