using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Randomizes the primary color palette
    /// </summary>
    internal class ColorPalettePixelRandomize : CEEvent
    {
        public ColorPalettePixelRandomize()
        {
            _name = "Too many mushrooms";
            _description = "Apparently taking too many of these can have side effects after all";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        Texture2D backupPalette;

        public override void StartupTrigger()
        {
            Room room = game.cameras[0].room;
            PlayerChangedRoomTrigger(ref game.cameras[0], ref room, ref game.cameras[0].currentCameraPosition);
            //Just in case the method touched the room ref
            game.cameras[0].room = room;
        }

        public override void PlayerChangedRoomTrigger(ref RoomCamera self, ref Room room, ref int camPos)
        {
            backupPalette = new Texture2D(self.fadeTexA.width, self.fadeTexA.height);
            backupPalette.SetPixels(self.fadeTexA.GetPixels());
            Texture2D texture = new Texture2D(32, 16, TextureFormat.ARGB32, false)
            {
                anisoLevel = 0,
                filterMode = FilterMode.Point
            };
            for (int i = 0; i < texture.height; i++)
            {
                for (int j = 0; j < texture.width; j++)
                {
                    texture.SetPixel(j, i, new Color(UnityEngine.Random.Range(0f, 0.75f), UnityEngine.Random.Range(0f, 0.75f), UnityEngine.Random.Range(0f, 0.75f)));
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
                cam.fadeTexA = backupPalette;
                backupPalette.Apply(false);
                cam.ApplyFade();
            }
        }
    }
}
