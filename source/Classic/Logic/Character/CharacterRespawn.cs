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

namespace Burntime.Classic.Logic
{
    [Serializable]
    public class CharacterRespawn : StateObject
    {
        [Serializable]
        protected class RespawnObject : StateObject
        {
            protected StateLink<Character> character;
            protected StateLink<Location> location;
            protected int remainingTime;

            public Character Character
            {
                get { return character; }
            }

            public Location Location
            {
                get { return location; }
            }

            public int RemainingTime
            {
                get { return remainingTime; }
            }

            protected override void InitInstance(object[] parameter)
            {
                if (parameter.Length != 3)
                    throw new Burntime.Framework.BurntimeLogicException();

                character = (Character)parameter[0];
                remainingTime = (int)parameter[1];
                this.location = (Location)parameter[2];

                base.InitInstance(parameter);
            }

            public void Turn()
            {
                remainingTime--;
            }
        }

        protected StateLinkList<RespawnObject> respawnList;
        protected int npcRespawn;
        protected int traderRespawn;
        protected int mutantRespawn;
        protected int dogRespawn;

        protected override void InitInstance(object[] parameter)
        {
            if (parameter.Length != 4)
                throw new Burntime.Framework.BurntimeLogicException();

            respawnList = container.CreateLinkList<RespawnObject>();
            npcRespawn = (int)parameter[0];
            traderRespawn = (int)parameter[1];
            mutantRespawn = (int)parameter[2];
            dogRespawn = (int)parameter[3];

            base.InitInstance(parameter);
        }

        public void Respawn(Character character)
        {
            // set time to respawn depending on character type;
            int timeToSpawn;
            switch (character.Class)
            {
                case CharClass.Trader:
                    timeToSpawn = traderRespawn;
                    break;
                case CharClass.Dog:
                    timeToSpawn = dogRespawn;
                    break;
                case CharClass.Mutant:
                    timeToSpawn = mutantRespawn;
                    break;
                default:
                    timeToSpawn = npcRespawn;
                    break;
            }

            // set for respawn in same location
            Location location = character.Location;

            // schedule for respawn
            respawnList.Add(container.Create<RespawnObject>(new object[] {character, timeToSpawn, location }));
        }

        public void Turn()
        {
            // update respawn list
            for (int i = 0; i < respawnList.Count; i++)
            {
                respawnList[i].Turn();

                // if remaing time is zero, then respawn
                if (respawnList[i].RemainingTime == 0)
                {
                    respawnList[i].Character.Revive();

                    // enter specified location
                    respawnList[i].Location.EnterLocation(respawnList[i].Character);

                    // remove from list
                    respawnList.Remove(respawnList[i]);
                    i--;
                }
            }
        }
    }
}
