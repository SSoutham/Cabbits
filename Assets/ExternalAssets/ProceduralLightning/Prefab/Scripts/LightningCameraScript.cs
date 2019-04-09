//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    [ExecuteInEditMode]
    public class LightningCameraScript : MonoBehaviour
    {
        public DepthTextureMode DepthTextureMode = DepthTextureMode.None;
        public Camera Camera;

        private void Update()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
                if (Camera == null)
                {
                    Camera = Camera.current;
                    if (Camera == null)
                    {
                        return;
                    }
                }
            }

            Camera.depthTextureMode = DepthTextureMode;
        }
    }
}