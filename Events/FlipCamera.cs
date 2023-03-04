using RainWorldCE.PostProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Flips the camera either by y
    /// </summary>
    internal class FlipCamera : CEEvent
    {
        public FlipCamera()
        {
            _name = "Camera issues";
            _description = "Maybe inverted controls would be useful now?";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
        }

        public override void StartupTrigger()
        {
            foreach (RoomCamera camera in game.cameras)
            {
                camera.AddPPEffect(new FlipScreenEffect());
            }
        }

        public override void ShutdownTrigger()
        {
            foreach (RoomCamera camera in game.cameras)
            {
                camera.RemovePPEffect(typeof(FlipScreenEffect));
            }
        }
    }

    public class FlipScreenEffect : IDrawable
    {
        float yFlip = 0;
        bool done;

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            rCam.ReturnFContainer("PostProcessing").AddChild(sLeaser.sprites[0]);
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (!done)
            {
                yFlip += 0.020f;
                Shader.SetGlobalFloat("Gamer025_YFlip", yFlip);

                if (yFlip > 1f)
                {
                    yFlip = 1f;
                    done = true;
                    Shader.SetGlobalFloat("Gamer025_YFlip", yFlip);
                }
            }

        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("Futile_White");
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["FlipScreenPP"];
            sLeaser.sprites[0].scaleX = rCam.game.rainWorld.options.ScreenSize.x / 16f;
            sLeaser.sprites[0].scaleY = 48f;
            sLeaser.sprites[0].anchorX = 0f;
            sLeaser.sprites[0].anchorY = 0f;
            AddToContainer(sLeaser, rCam, null);
        }
    }
}


