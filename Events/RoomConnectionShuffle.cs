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
            //Find random room that isn't our room
            AbstractRoom targetRoom;
            do
                targetRoom = EventHelpers.RandomRegionRoom();
            while (targetRoom.index == room.abstractRoom.index);

            //Just always backup the connections and only restore if config set
            if (!roomBackup.ContainsKey(targetRoom))
                roomBackup.Add(targetRoom, (int[])targetRoom.connections.Clone());
            if (!roomBackup.ContainsKey(room.abstractRoom))
                roomBackup.Add(room.abstractRoom, (int[])room.abstractRoom.connections.Clone());

            //Does this https://cdn.discordapp.com/attachments/484188983061118977/1000807292154945606/unknown.png
            int targetRoomNodeIndex = rnd.Next(0, targetRoom.connections.Length);
            AbstractRoom targetRoomOrigDestRoom = room.world.GetAbstractRoom(targetRoom.connections[targetRoomNodeIndex]);
            if (!roomBackup.ContainsKey(targetRoomOrigDestRoom))
                roomBackup.Add(targetRoomOrigDestRoom, (int[])targetRoomOrigDestRoom.connections.Clone());
            int targetRoomOrigDestRoomNodeIndex = targetRoomOrigDestRoom.ExitIndex(targetRoom.index);
            AbstractRoom ourRoomOrigDestRoom = room.world.GetAbstractRoom(room.abstractRoom.connections[shortCut.destNode]);
            if (!roomBackup.ContainsKey(ourRoomOrigDestRoom))
                roomBackup.Add(ourRoomOrigDestRoom, (int[])ourRoomOrigDestRoom.connections.Clone());
            int ourRoomOrigDestRoomNodeIndex = ourRoomOrigDestRoom.ExitIndex(room.abstractRoom.index);
            targetRoom.connections[targetRoomNodeIndex] = room.abstractRoom.index;
            room.abstractRoom.connections[shortCut.destNode] = targetRoom.index;
            targetRoomOrigDestRoom.connections[targetRoomOrigDestRoomNodeIndex] = ourRoomOrigDestRoom.index;
            ourRoomOrigDestRoom.connections[ourRoomOrigDestRoomNodeIndex] = targetRoomOrigDestRoom.index;
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
                List<EventConfigEntry> options = new List<EventConfigEntry>
                {
                    new BooleanConfigEntry("Restore connections?", "Restore room connection at end of event?", "restoreConnections", true, this)
                };
                return options;
            }
        }
    }
}
