using BepInEx.Logging;
using RainWorldCE.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Shuffles pipe connections of the room whenever the player is about to change room
    /// </summary>
    internal class RoomConnectionShuffle : CEEvent
    {
        public RoomConnectionShuffle()
        {
            _name = "Geographic issues";
            _description = "Didn't I just come from here?";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        private readonly Dictionary<AbstractRoom, int[]> roomBackup = new Dictionary<AbstractRoom, int[]>();

        public override void PlayerChangingRoomTrigger(ref ShortcutHandler self, ref Creature creature, ref Room room, ref ShortcutData shortCut)
        {
            AbstractRoom targetRoom = helper.RandomRegionRoom;

            if (TryGetConfigAsBool("restoreConnections") && !roomBackup.ContainsKey(targetRoom))
            {
                roomBackup.Add(targetRoom, (int[])targetRoom.connections.Clone());
            }
            //Does this https://cdn.discordapp.com/attachments/484188983061118977/1000807292154945606/unknown.png
            //connections == 0 normally shouldn't happen because RandomRegionRoom excludes the last room, which should be the offscreenDen but just in case
            //Also messing up GATE connections is probably a bad idea so lets not do it
            if (targetRoom.connections.Length != 0 && !targetRoom.name.Contains("GATE"))
            {
                int targetRoomNodeIndex = rnd.Next(0, targetRoom.connections.Length);
                AbstractRoom targetRoomOrigDestRoom = room.world.GetAbstractRoom(targetRoom.connections[targetRoomNodeIndex]);
                int targetRoomOrigDestRoomNodeIndex = targetRoomOrigDestRoom.ExitIndex(targetRoom.index);
                AbstractRoom ourRoomOrigDestRoom = room.world.GetAbstractRoom(room.abstractRoom.connections[shortCut.destNode]);
                int ourRoomOrigDestRoomNodeIndex = ourRoomOrigDestRoom.ExitIndex(room.abstractRoom.index);
                targetRoom.connections[targetRoomNodeIndex] = room.abstractRoom.index;
                room.abstractRoom.connections[shortCut.destNode] = targetRoom.index;
                targetRoomOrigDestRoom.connections[targetRoomOrigDestRoomNodeIndex] = ourRoomOrigDestRoom.index;
                ourRoomOrigDestRoom.connections[ourRoomOrigDestRoomNodeIndex] = targetRoomOrigDestRoom.index;
            }
        }

        public override void ShutdownTrigger()
        {
            //Restore all backed up room connections if config enabled
           if (TryGetConfigAsBool("restoreConnections"))
            {
                foreach (var room in game.world.abstractRooms.Where(room => roomBackup.ContainsKey(room)))
                {
                    room.connections = (int[])roomBackup[room].Clone();
                }
            }
        }

        public override List<EventConfigEntry> ConfigEntries
        {
            get
            {
                List<EventConfigEntry> options = new List<EventConfigEntry>();
                options.Add(new BooleanConfigEntry("Restore connections?", "Restore room connection at end of event?", "restoreConnections", this));
                return options;
            }
        }
    }
}
