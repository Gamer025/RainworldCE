using RainWorldCE.Config;
using RainWorldCE.PostProcessing;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Pixelizes the screen
    /// </summary>
    internal class Pixelize : CEEvent
    {
        public Pixelize()
        {
            _name = "Retro";
            _description = "Rain World finally arrived on the NES";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        public override void StartupTrigger()
        {
            foreach (RoomCamera camera in game.cameras)
            {
                camera.AddPPEffect(new PixelizeEffect());
            }
            int intensity = TryGetConfigAsInt("intensity");
            Shader.SetGlobalFloat("Gamer025_PixelizeIntens", Mathf.Lerp(1.5f, 10f, Mathf.InverseLerp(10, 100, intensity)));
        }

        public override void ShutdownTrigger()
        {
            foreach (RoomCamera camera in game.cameras)
            {
                camera.RemovePPEffect(typeof(PixelizeEffect));
            }
        }
        public override List<EventConfigEntry> ConfigEntries
        {
            get
            {
                List<EventConfigEntry> options = new List<EventConfigEntry>
                {
                    new IntegerConfigEntry("Intensity", "How badly the game will be retrofied", "intensity", new RWCustom.IntVector2(10, 100), 30, this)
                };
                return options;
            }
        }
    }


    public class PixelizeEffect : IDrawable
    {
        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            rCam.ReturnFContainer("PostProcessing").AddChild(sLeaser.sprites[0]);
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {

        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("Futile_White");
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["PixelizePP"];
            sLeaser.sprites[0].scaleX = rCam.game.rainWorld.options.ScreenSize.x / 16f;
            sLeaser.sprites[0].scaleY = 48f;
            sLeaser.sprites[0].anchorX = 0f;
            sLeaser.sprites[0].anchorY = 0f;
            AddToContainer(sLeaser, rCam, null);
        }
    }
}


