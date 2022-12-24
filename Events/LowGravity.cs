using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SlugcatStats;
using UnityEngine;
using System.CodeDom;

namespace RainWorldCE.Events
{
    internal class LowGravity : CEEvent
    {
        public LowGravity()
        {
            _name = "Low gravity";
            _description = "Try to not hit your head";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        private readonly Dictionary<Room, float> affectedRooms = new Dictionary<Room, float>();

        public override void StartupTrigger()
        {
            AddZeroGToRoom(EventHelpers.CurrentRoom.realizedRoom);
        }
        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            AddZeroGToRoom(room);
        }

        private void AddZeroGToRoom(Room room)
        {
            //Only rooms without special gravity
            if (EventHelpers.RoomHasEffect(room, RoomSettings.RoomEffect.Type.ZeroG)
                || EventHelpers.RoomHasEffect(room, RoomSettings.RoomEffect.Type.BrokenZeroG)
                || affectedRooms.ContainsKey(room)) return;

            affectedRooms.Add(room, room.gravity);
            room.gravity = 0.2f;
        }

        public override void ShutdownTrigger()
        {
            //Set back gravity to previous value on event end
            foreach (KeyValuePair<Room,float> KVP in affectedRooms)
            {
                KVP.Key.gravity = KVP.Value;
            }
        }
    }
}
