using RainWorldCE.Config;
using System.Collections.Generic;
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
            _description = "Darkness has fallen";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        float amount = 0.8f;

        public override void StartupTrigger()
        {
            _name = $"Shaded {Region.GetRegionFullName(game.world.region.name, (EventHelpers.MainPlayer.realizedCreature as Player).SlugCatClass)}";
            amount = TryGetConfigAsInt("amount") / 100f;
            Room room = game.cameras[0].room;
            PlayerChangedRoomTrigger(ref game.cameras[0], ref room, ref game.cameras[0].currentCameraPosition);
            //Just in the method touched the room ref
            game.cameras[0].room = room;
        }

        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
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
                    new IntegerConfigEntry("Darkness", "How much more darker the rooms are made", "amount", new RWCustom.IntVector2(10, 100), 80, this)
                };
                return options;
            }
        }

    }
}
