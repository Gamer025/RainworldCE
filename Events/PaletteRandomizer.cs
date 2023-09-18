using BepInEx.Logging;
using RainWorldCE.Config;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Randomizes the primary color palette
    /// </summary>
    internal class PaletteRandomizer : CEEvent
    {
        public PaletteRandomizer()
        {
            _name = "Too many mushrooms";
            _description = "Apparently taking too many of these can have side effects after all";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        public override void StartupTrigger()
        {
            Room room = game.cameras[0].room;
            PlayerChangedRoomTrigger(ref game.cameras[0], ref room, ref game.cameras[0].currentCameraPosition);
            //Just in case the method touched the room ref
            game.cameras[0].room = room;
        }

        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            int chance = TryGetConfigAsInt("chance");
            Texture2D texture = new Texture2D(32, 16, TextureFormat.ARGB32, false)
            {
                anisoLevel = 0,
                filterMode = FilterMode.Point
            };
            texture.SetPixels(self.fadeTexA.GetPixels());
            for (int i = 0; i < texture.height; i++)
            {
                for (int j = 0; j < texture.width; j++)
                {
                    if (rnd.Next(100) < chance)
                    {
                        texture.SetPixel(j, i, new Color(UnityEngine.Random.Range(0f, 0.75f), UnityEngine.Random.Range(0f, 0.75f), UnityEngine.Random.Range(0f, 0.75f)));
                    }
                }
            }
            WriteLog(LogLevel.Debug, $"Applying random color palette");
            self.ApplyEffectColorsToPaletteTexture(ref texture, room.roomSettings.EffectColorA, room.roomSettings.EffectColorB);
            self.fadeTexA = texture;
            texture.Apply(false);
            self.ApplyFade();
        }

        public override void ShutdownTrigger()
        {
            foreach (RoomCamera cam in game.cameras)
            {
                Texture2D restore = new Texture2D(cam.fadeTexA.width, cam.fadeTexA.height)
                {
                    anisoLevel = 0,
                    filterMode = FilterMode.Point
                };
                cam.LoadPalette(cam.room.roomSettings.Palette, ref restore);
                cam.ApplyEffectColorsToPaletteTexture(ref restore, cam.room.roomSettings.EffectColorA, cam.room.roomSettings.EffectColorB);
                cam.fadeTexA = restore;
                restore.Apply(false);
                cam.ApplyFade();
            }
        }

        public override List<EventConfigEntry> ConfigEntries
        {
            get
            {
                List<EventConfigEntry> options = new List<EventConfigEntry>
                {
                    new IntegerConfigEntry("Intensity", "How badly the palette will be randomized", "chance", new RWCustom.IntVector2(10, 100), 25, this)
                };
                return options;
            }
        }

    }
}
