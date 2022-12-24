using BepInEx.Logging;
using RainWorldCE.Config;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Changes the water level of upcoming rooms to a random amount
    /// </summary>
    internal class WaterRising : CEEvent
    {
        public WaterRising()
        {
            _name = "Water rising";
            _description = "Seems like drainage system is clogged again";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        private readonly List<Room> modifiedRooms = new List<Room>();
        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            if (TryGetConfigAsBool("restoreWater") && !modifiedRooms.Contains(room))
            {
                modifiedRooms.Add(room);
            }
            if (room.waterObject == null)
            {
                room.AddWater();

            }
            room.waterObject.fWaterLevel = room.MiddleOfTile(new IntVector2(0, (int)(room.Height * UnityEngine.Random.Range(0.2f, 0.6f)))).y;
        }

        public override void ShutdownTrigger()
        {
            if (TryGetConfigAsBool("restoreWater"))
            {
                foreach (Room room in modifiedRooms)
                {
                    foreach (UpdatableAndDeletable thing in room.updateList)
                    {
                        if (thing.GetType() == typeof(Water.WaterSoundObject)) room.RemoveObject(thing);
                    }
                    foreach (RoomCamera cam in game.cameras)
                    {
                        if (cam.waterLight != null) cam.waterLight.CleanOut();
                        cam.waterLight = null;
                        for (int i = cam.spriteLeasers.Count - 1; i >= 0; i--)
                        {
                            RoomCamera.SpriteLeaser leaser = cam.spriteLeasers[i];
                            if (leaser.drawableObject == room.waterObject)
                            {
                                WriteLog(LogLevel.Debug, $"Found {leaser}, calling CleanSpritesAndRemove");
                                leaser.CleanSpritesAndRemove();
                            }
                        }

                    }

                    room.drawableObjects.Remove(room.waterObject);
                    room.waterObject = null;
                    room.water = false;
                    if (room.defaultWaterLevel > 0)
                    {
                        room.AddWater();
                    }

                }
            }
        }

        public override List<EventConfigEntry> ConfigEntries
        {
            get
            {
                List<EventConfigEntry> options = new List<EventConfigEntry>
                {
                    new BooleanConfigEntry("Restore water levels?", "Restore water levels at end of event?", "restoreWater", false, this)
                };
                return options;
            }
        }
    }
}
