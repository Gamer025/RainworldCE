using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RainWorldCE.Events
{
    //Randomizes the players colors everytime palettes are applied / every 10 secs
    internal class RainbowCat : CEEvent
    {
        public RainbowCat()
        {
            _name = "Rainbow Cat";
            _description = "*May include non rainbow colors";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
            _repeatEverySec = 10;
        }

        public override void StartupTrigger()
        {
            On.PlayerGraphics.ApplyPalette += ApplyPaletteHook;
            game.cameras[0].ApplyPalette();
        }

        public override void ShutdownTrigger()
        {
            On.PlayerGraphics.ApplyPalette -= ApplyPaletteHook;
            game.cameras[0].ApplyPalette();
        }

        //Change the players color every 10 second by causing a palette update
        public override void RecurringTrigger()
        {
            foreach (RoomCamera cam in game.cameras)
            {
                cam.ApplyPalette();
            }

        }

        public void ApplyPaletteHook(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            Color body =  new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            foreach (FSprite sprite in sLeaser.sprites)
            {
                sprite.color = body;
            }
            sLeaser.sprites[9].color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            sLeaser.sprites[10].color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            sLeaser.sprites[11].color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }

    }
}
