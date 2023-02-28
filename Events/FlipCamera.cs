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
    /// Flips the camera either by y or x
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
           foreach (Camera camera in Camera.allCameras)
            {
                camera.gameObject.AddComponent<FlipShaderRender>();
            }
        }

        public override void ShutdownTrigger()
        {
            foreach (Camera camera in Camera.allCameras)
            {
                var behaviour = camera.gameObject.GetComponent<FlipShaderRender>();
                Component.Destroy(behaviour);
            }
        }
    }

    public class FlipShaderRender : MonoBehaviour
    {
        // Start is called before the first frame update
        private Material material;

        // Creates a private material used to the effect
        void Awake()
        {
           
            var shader = RainWorldCE.CEAssetBundle.LoadAsset<Shader>("flipscreen.shader");
            material = new Material(shader);
        }

        float yFlip = 0;
        bool done;
        // Postprocess the image
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!done)
            {
                yFlip += 0.020f;
                material.SetFloat("_FlipY", yFlip);

                if (yFlip > 1f)
                {
                    yFlip = 1f;
                    done = true;
                    material.SetFloat("_FlipY", yFlip);
                }
            }
            
            
            Graphics.Blit(source, destination, material);
        }
    }
}


