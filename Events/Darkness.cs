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
    /// Makes the current color palette darker
    /// </summary>
    internal class Darkness : CEEvent
    {
        public Darkness()
        {
            _name = "Darkness / Shaded Region";
            if (EventHelpers.StoryModeActive && game.world.region is not null)
            {
                string regionName = game.world.region.name switch
                {
                    "CC" => "Chimney Canopy",
                    "DS" => "Drainage System",
                    "HI" => "Industrial Complex",
                    "GW" => "Garbage Wastes",
                    "SI" => "Sky Islands",
                    "SU" => "Outskirts",
                    "SH" => "Citadel",
                    "SL" => "Shoreline",
                    "LF" => "Farm Arrays",
                    "UW" => "The Exterior",
                    "SB" => "Subterranean",
                    "SS" => "Five Pebbles",
                    _ => "Region"
                };

                _name = $"Shaded {regionName}";
            }
            _description = "Darkness has fallen";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        Texture2D backupPalette;
        float amount = 0.8f;

        public override void StartupTrigger()
        {
            amount = (float)TryGetConfigAsInt("amount") / 100f;
            Room room = game.cameras[0].room;
            PlayerChangedRoomTrigger(ref game.cameras[0], ref room, ref game.cameras[0].currentCameraPosition);
            //Just in the method touched the room ref
            game.cameras[0].room = room;
        }

        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            backupPalette = new Texture2D(self.fadeTexA.width, self.fadeTexA.height)
            {
                anisoLevel = 0,
                filterMode = FilterMode.Point
            };
            backupPalette.SetPixels(self.fadeTexA.GetPixels());
            Texture2D texture = new Texture2D(32, 16, TextureFormat.ARGB32, false)
            {
                anisoLevel = 0,
                filterMode = FilterMode.Point
            };
            Texture2D darkPal = new Texture2D(32, 16, TextureFormat.ARGB32, false);
            self.LoadPalette(10, ref darkPal);

            texture.SetPixels(self.fadeTexA.GetPixels());
            Color[] normalColors = texture.GetPixels();
            Color[] darkColors = darkPal.GetPixels();
            Color[] mixed = new Color[normalColors.Length];

            for (int i = 0; i < normalColors.Length; i++)
            {
                Color normalColor = normalColors[i];
                Color darkColor = darkColors[i];
                mixed[i] = Color.Lerp(normalColor, darkColor, amount);
            }

            texture.SetPixels(mixed);
            //Darkness pixels
            texture.SetPixel(30, 7, new Color(0, 0, 0));
            texture.SetPixel(31, 7, new Color(255, 255, 255));

            self.ApplyEffectColorsToPaletteTexture(ref texture, room.roomSettings.EffectColorA, room.roomSettings.EffectColorB);
            self.fadeTexA = texture;
            texture.Apply(false);
            self.ApplyFade();
        }

        public override void ShutdownTrigger()
        {
            foreach (RoomCamera cam in game.cameras)
            {
                cam.fadeTexA = backupPalette;
                backupPalette.Apply(false);
                cam.ApplyFade();
            }
        }

        public override List<EventConfigEntry> ConfigEntries
        {
            get
            {
                List<EventConfigEntry> options = new List<EventConfigEntry>
                {
                    new IntegerConfigEntry("Darkness", "How much more darker the rooms are made", "amount", new RWCustom.IntVector2(10, 100), 80, this)
                };
                return options;
            }
        }

    }
}
