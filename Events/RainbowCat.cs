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

        Color primary;
        Color secondary;

        public override void StartupTrigger()
        {
            primary = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            secondary = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            On.PlayerGraphics.ApplyPalette += ApplyPaletteHook;
            foreach (RoomCamera cam in game.cameras)
            {
                cam.ApplyPalette();
            }
        }

        public override void ShutdownTrigger()
        {
            On.PlayerGraphics.ApplyPalette -= ApplyPaletteHook;
            game.cameras[0].ApplyPalette();
        }

        //Change the players color every 10 second by causing a palette update
        public override void RecurringTrigger()
        {
            primary = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            secondary = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            foreach (RoomCamera cam in game.cameras)
            {
                cam.ApplyPalette();
            }

        }

        public override void PlayerChangingRoomTrigger(ref ShortcutHandler self, ref Creature creature, ref Room room, ref ShortcutData shortCut)
        {
            primary = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            secondary = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }

        public void ApplyPaletteHook(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            foreach (FSprite sprite in sLeaser.sprites)
            {
                sprite.color = primary;
            }
            sLeaser.sprites[9].color = secondary;
            sLeaser.sprites[10].color = secondary;
            sLeaser.sprites[11].color = secondary;
        }

    }
}
