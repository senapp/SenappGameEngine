using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Models;
using Senapp.Engine.Utilities;
using Senapp.Engine.Entities;
using Senapp.Engine.Shaders.Components;
using Senapp.Engine.Utilities.Testing;
using System.Drawing;
using Senapp.Engine.Core.Transforms;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.Loaders;
using Senapp.Engine.Core;
using Senapp.Engine.Core.GameObjects;
using OpenTK;
using System;
using System.Diagnostics;

namespace Senapp.Engine.Renderer.ComponentRenderers
{
    public class GrassRenderer
    {
        const int GRASS_BLADES = 500;


        RawModel grassModel;
        Transform pos;
        Texture texture;
        public GrassRenderer()
        {
            float width = 0.75f;
            float height = 1f;
            texture = Loader.LoadTexture("grass_blade");
            shader = new GrassShader();
            //grassModel = Loader.LoadToVAO(new LoaderVertex(
            //    new float[]
            //    {
            //        -0.4f * width, 1.5f * height, 0,
            //        -0.45f* width, 0f* height, 0,
            //        0.45f* width, 0f* height, 0,
            //        0.4f* width, 1.5f* height, 0,
            //        -0.3f* width, 3f* height, 0,
            //        0.3f* width, 3f* height, 0,
            //         -0.2f* width, 4.5f* height, 0,
            //        0.2f* width, 4.5f* height, 0,
            //         0* width, 6f* height, 0,
            //    },
            //    new float[]
            //    {
            //        0,0.75f,
            //        0,1,
            //        1,1,
            //        1,0.75f,
            //
            //        0,0.5f,
            //        1,0.5f,
            //
            //        0,0.25f,
            //        1,0.25f,
            //
            //        0.5f,0,
            //    },
            //    new float[]
            //    {
            //        0,0.25f,0.5f,
            //        0,0,0.5f,
            //        1,0,0.5f,
            //        1,0.25f,0.5f,
            //
            //        0,0.5f,0.5f,
            //        1,0.5f,0.5f,
            //
            //        0,0.75f,0.5f,
            //        1,0.75f,0.5f,
            //
            //        0.5f,1,0.5f,
            //    }, new int[]
            //    {
            //        0,1,2,
            //        3,0,2,
            //        4,0,3,
            //        5,4,3,
            //        6,4,5,
            //        7,6,5,
            //        8,6,7
            //    }
            //    ), "Grass");
            grassModel = Loader.LoadToVAO(new LoaderVertex(
                new float[]
                {
                    -0.3f, 1f, 0,
                    -0.3f, 0f, 0,
                    0.3f, 0f, 0,
                    0.3f, 1f, 0,

                    -0.26f, 1.8f, 0,
                    0.26f, 1.8f, 0,

                    -0.21f, 2.6f, 0,
                    0.21f, 2.6f, 0,

                     -0.16f, 3.3f, 0,
                    0.16f, 3.3f, 0,

                     -0.1f, 4f, 0,
                    0.1f, 4f, 0,

                     0, 4.5f, 0,
                },
                new float[]
                {
                    0,0.83f,
                    0,1,
                    1,1,
                    1,0.83f,
            
                    0,0.66f,
                    1,0.66f,

                    0,0.49f,
                    1,0.49f,

                    0,0.32f,
                    1,0.32f,

                    0,0.15f,
                    1,0.15f,
            
                    0.5f,0,
                },
                new float[]
                {
                    0,0.25f,0.5f,
                    0,0,0.5f,
                    1,0,0.5f,
                    1,0.25f,0.5f,
            
                    0,0.5f,0.5f,
                    1,0.5f,0.5f,
            
                    0,0.75f,0.5f,
                    1,0.75f,0.5f,
            
                    0.5f,1,0.5f,

                    0,0.83f,0.5f,
                    0,1,0.5f,
                    1,1,0.5f,
                    1,0.83f,0.5f,

                    0,0.66f,0.5f,
                    1,0.66f,0.5f,
                           
                    0,0.49f,0.5f,
                    1,0.49f,0.5f,
                           
                    0,0.32f,0.5f,
                    1,0.32f,0.5f,
                           
                    0,0.15f,0.5f,
                    1,0.15f,0.5f,

                    0.5f,0,0.5f,
                }, new int[]
                {
                    0,1,2,
                    3,0,2,

                    4,0,3,
                    5,4,3,

                    6,4,5,
                    7,6,5,

                    8,6,7,
                    9,8,7,

                    10,8,9,
                    11,10,9,

                    12,10,11,
                }
                ), "Grass");
            pos = new Transform(null);
            pos.SetScale(new Vector3(0.08f));
            // pos.SetRotation(new Vector3(0, 90, 0));
        }

        public void Render(bool isColourPass, Light light, Camera camera, GameObject grassBed)
        {
            shader.Start();
            shader.LoadCameraMatrix(camera);
            shader.LoadGrassGrid(GRASS_BLADES, GRASS_BLADES);
            shader.LoadIsColourPass(isColourPass);
            shader.LoadCameraPosition(camera.gameObject.transform.GetWorldPosition());
            shader.LoadLight(light);

            var time = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            shader.LoadTime((float)time.TotalMilliseconds / 1000f);

            var wireFrame = WireFrame.IsEnabled();
            if (wireFrame) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            MasterRenderer.DisableCulling();
            BindCommon(grassModel);

            PrepareBed(pos);
            GL.DrawElementsInstanced(PrimitiveType.Triangles, grassModel.VertexCount, DrawElementsType.UnsignedInt, 0, GRASS_BLADES * GRASS_BLADES);
            UnbindCommon();
            MasterRenderer.EnableCulling();

            if (wireFrame) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            shader.Stop();
        }

        public void BindCommon(RawModel model)
        {
            texture.Bind(TextureUnit.Texture0);
            GL.BindVertexArray(model.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
        }
        public void UnbindCommon() 
        {
            GL.ActiveTexture(0);
            GL.BindTexture(TextureTarget.Texture2D, 0); 
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.BindVertexArray(0);

            MasterRenderer.EnableCulling();
        }
        public void PrepareBed(Transform transform)
        {
            shader.LoadTransformationMatrix(transform.CalculateTransformationMatrix());
        }
        public void Dispose()
        {
            shader.Dispose();
        }

        private readonly GrassShader shader;
    }
}
