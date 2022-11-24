﻿using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    internal class NoodleInvasion : CEEvent
    {
        public NoodleInvasion()
        {
            _name = "Noodle invasion";
            _description = "Free food, but for whom?";
        }

        public override void StartupTrigger()
        {
            float chance = 100;
            float interpolation = 0.1f;
            List<AbstractRoom> adjancedRooms = helper.GetConnectedRooms(helper.CurrentRoom);
            foreach (AbstractRoom room in adjancedRooms)
            {
                chance = UnityEngine.Mathf.Lerp(chance, 0f, interpolation);
                interpolation += 0.25f;
                WriteLog(LogLevel.Debug, $"Chance now {chance}");
                if (rnd.Next(100) < chance)
                {
                    int destRoomNodeId = helper.GetNodeIdOfRoomConnection(room, helper.CurrentRoom);
                    WriteLog(LogLevel.Debug, $"Spawned noodles in {room.name}");
                    AbstractCreature creature1 = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.SmallNeedleWorm), null, new WorldCoordinate(room.index, -1, -1, 0), game.GetNewID());
                    AbstractCreature creature2 = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.SmallNeedleWorm), null, new WorldCoordinate(room.index, -1, -1, 0), game.GetNewID());
                    AbstractCreature creature3 = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.BigNeedleWorm), null, new WorldCoordinate(room.index, -1, -1, 0), game.GetNewID());
                    helper.MakeCreatureAttackCreature(creature3, helper.MainPlayer);
                    creature1.ChangeRooms(new WorldCoordinate(helper.CurrentRoom.index, -1, -1, destRoomNodeId));
                    creature2.ChangeRooms(new WorldCoordinate(helper.CurrentRoom.index, -1, -1, destRoomNodeId));
                    creature3.ChangeRooms(new WorldCoordinate(helper.CurrentRoom.index, -1, -1, destRoomNodeId));
                }
            }
        }
    }
}
