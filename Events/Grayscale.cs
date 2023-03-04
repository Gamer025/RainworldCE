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
    /// <summary>
    /// Makes the room grayscale by adding the Desaturation room effect
    /// </summary>
    internal class Grayscale : CEEvent
    {
        public Grayscale()
        {
            _name = "Gray World";
            _description = "Can slugs even see colors?";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        private readonly List<Room> affectedRooms = new List<Room>();

        public override void StartupTrigger()
        {
            AddDesaturation(EventHelpers.CurrentRoom.realizedRoom);
        }
        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            AddDesaturation(room);
        }

        private void AddDesaturation(Room room)
        {
            //Only once
            if (affectedRooms.Contains(room)) return;

            affectedRooms.Add(room);
            room.roomSettings.effects.Add(new RoomSettings.RoomEffect(RoomSettings.RoomEffect.Type.Desaturation, 1f, false));
        }

        public override void ShutdownTrigger()
        {
            //Set back gravity to previous value on event end
            foreach (Room room in affectedRooms)
            {
                room.roomSettings.effects = room.roomSettings.effects.Where(x => x.type != RoomSettings.RoomEffect.Type.Desaturation).ToList();
            }
        }
    }
}
