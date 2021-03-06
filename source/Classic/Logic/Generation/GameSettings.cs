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

using Burntime.Platform.IO;

namespace Burntime.Classic.Logic.Generation
{
    class GameSettings
    {
        public struct RespawnTimes
        {
            public int NPC;
            public int Trader;
            public int Dog;
            public int Mutant;
        }

        public struct ItemGeneration
        {
            public string[] Include;
            public string[] Exclude;
            public int Minimum;
            public int Maximum;

            public int RandomCount
            {
                get
                {
                    if (Minimum == Maximum)
                        return Minimum;
                    return Burntime.Platform.Math.Random.Next() % (Maximum - Minimum) + Minimum;
                }
            }
        }

        ConfigFile config;
        string difficulty;
        RespawnTimes respawn;
        AI.AiSettings ai;

        public string[] StartItems
        {
            get { return config[difficulty].GetStrings("start_items"); }
        }

        public int[] StartLocations
        {
            get { return config[difficulty].GetInts("start_locations"); }
        }

        public int StartExperience
        {
            get { return config[difficulty].GetInt("start_experience"); }
        }

        public string[] RandomItems
        {
            get { return config[difficulty].GetStrings("random_items"); }
        }

        public int RandomItemsMin
        {
            get { return config[difficulty].GetInt("random_items_rate_min"); }
        }

        public int RandomItemsMax
        {
            get { return config[difficulty].GetInt("random_items_rate_max"); }
        }

        public RespawnTimes Respawn
        {
            get { return respawn; }
        }

        public int StartHealth
        {
            get { return 100; }
        }

        public int StartFood
        {
            get { return 9; }
        }

        public int StartWater
        {
            get { return 5; }
        }

        public AI.AiSettings AiSettings
        {
            get { return ai; }
        }

        public GameSettings(string file)
        {
            config = new ConfigFile();
            config.Open(file);
        }

        public void SetDifficulty(int difficulty)
        {
            this.difficulty = difficulty.ToString();

            respawn.NPC = config[this.difficulty].GetInt("npc_respawn");
            respawn.Trader = config[this.difficulty].GetInt("trader_respawn");
            respawn.Mutant = config[this.difficulty].GetInt("mutant_respawn");
            respawn.Dog = config[this.difficulty].GetInt("dog_respawn");

            ai.MinInterval = config[this.difficulty].GetInts("ai_camp_interval")[0];
            ai.MaxInterval = config[this.difficulty].GetInts("ai_camp_interval")[1];
            ai.MaxAdvance = config[this.difficulty].GetInt("ai_camp_max_advance");
        }

        public ItemGeneration GetItemGeneration(string name)
        {
            string[] itemclass = config[difficulty].GetStrings(name);
            List<string> include = new List<string>();
            List<string> exclude = new List<string>();

            ItemGeneration generation;

            foreach (string item in itemclass)
            {
                if (item.StartsWith("-"))
                    exclude.Add(item.Substring(1));
                else
                    include.Add(item);
            }

            generation.Include = include.ToArray();
            generation.Exclude = exclude.ToArray();

            int[] rate = config[difficulty].GetInts(name + "_rate");

            generation.Minimum = rate.Length > 0 ? rate[0] : 0;
            generation.Maximum = rate.Length > 1 ? rate[1] : generation.Minimum;

            return generation;
        }
    }
}
