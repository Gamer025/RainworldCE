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
    /// Screen melting effect
    /// </summary>
    internal class Melting : CEEvent
    {
        public Melting()
        {
            _name = "Extreme Heat";
            _description = "Everything melts, even the nacho cheese";
            _repeatEverySec = 2;
            _activeTime = 80;
        }

        private Texture2D MeltText;
        private Color[] meltPixels;
        public override void StartupTrigger()
        {
            MeltText = new Texture2D(Screen.width, 1)
            {
                filterMode = FilterMode.Point
            };
            meltPixels = new Color[Screen.width];
            for (int i = 0; i < meltPixels.Length; i++)
            {
                meltPixels[i] = new Color(0, 0, 0, 0);
            }
            MeltText.SetPixels(meltPixels);
            MeltText.Apply();
            Shader.SetGlobalTexture("Gamer025_MeltTex", MeltText);
            foreach (RoomCamera camera in game.cameras)
            {
                camera.AddPPEffect(new MeltEffect());
            }
        }

        public override void ShutdownTrigger()
        {
            foreach (RoomCamera camera in game.cameras)
            {
                camera.RemovePPEffect(typeof(MeltEffect));
            }
        }

        public override void RecurringTrigger()
        {
            for (int i = 0; i < meltPixels.Length; i++)
            {
                if (UnityEngine.Random.Range(1, 1000) > 985) //97
                {
                    float amt = UnityEngine.Random.Range(1, 7); //6
                    meltPixels[i].r += amt / 255f;
                    for (int away = 1; away < amt; away++)
                    {
                        if (i + away < meltPixels.Length)
                        {
                            meltPixels[i + away].r += (amt - away) / 255f;
                        }
                        if (i - away >= 0)
                        {
                            meltPixels[i - away].r += (amt - away) / 255f;
                        }
                    }
                }
            }
            MeltText.SetPixels(meltPixels);
            MeltText.Apply();
        }
    }

    public class MeltEffect : IDrawable
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
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["MeltingPP"];
            sLeaser.sprites[0].scaleX = rCam.game.rainWorld.options.ScreenSize.x / 16f;
            sLeaser.sprites[0].scaleY = 48f;
            sLeaser.sprites[0].anchorX = 0f;
            sLeaser.sprites[0].anchorY = 0f;
            AddToContainer(sLeaser, rCam, null);
        }
    }
}


