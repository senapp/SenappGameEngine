using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.ComponentRenderers;
using Senapp.Engine.Renderer.FrameBuffers;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Renderer
{
    public class MasterRenderer
    {
        public static void ClearScreen()    
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        public static void EnableCulling()
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }
        public static void DisableCulling()
        {
            GL.Disable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        public readonly EntityRenderer entityRenderer;
        public readonly SpriteRenderer spriteRenderer;
        public readonly TextRenderer textRenderer;
        public readonly SkyboxRenderer skyboxRenderer;
        public readonly LightingRenderer lightingRenderer;
        public readonly FinalRenderer finalRenderer;

        public MasterRenderer(Camera camera)
        {
            GL.Ortho(0, Game.Instance.Width, Game.Instance.Height, 0, 0, 1);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);

            EnableCulling();

            entityRenderer = new EntityRenderer();
            spriteRenderer = new SpriteRenderer();
            textRenderer = new TextRenderer();
            skyboxRenderer = new SkyboxRenderer(camera.GetProjectionMatrix());
            lightingRenderer = new LightingRenderer();
            finalRenderer = new FinalRenderer();

            SetupFrameBuffers();
        }

        public void OnScreenResize(int screenWidth, int screenHeight)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, screenWidth, screenHeight, 0.0, 1.0, -1.0);

            var ratioX = Game.Instance.Width / (float)screenWidth;
            var ratioY = Game.Instance.Height / (float)screenHeight;
            var ratio = ratioX < ratioY ? ratioX : ratioY;
            var viewWidth = Convert.ToInt32(screenWidth * ratio);
            var viewHeight = Convert.ToInt32(screenHeight * ratio);
            var viewX = Convert.ToInt32((Game.Instance.Width - screenWidth * ratio) / 2);
            var viewY = Convert.ToInt32((Game.Instance.Height - screenHeight * ratio) / 2);

            GL.Viewport(viewX, viewY, viewWidth, viewHeight);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            RecalculateSize(Game.Instance.Width, Game.Instance.Height);
        }
        public void RecalculateSize(int width, int height)
        {
            geometryColourFbo?.Resize(width, height);
            geometryDataFbo?.Resize(width, height);
            finalFbo?.Resize(width, height);
        }

        public void Render(Light sun, Camera camera)
        {
            #region Process gameobjects
            foreach (var gameObject in Game.Instance.GetSceneGameObjects())
            {
                gameObject.ProccessRenderHierarchy<Entity>(ProcessEntity);
                gameObject.ProccessRenderHierarchy<Sprite>(ProcessSprite);
                gameObject.ProccessRenderHierarchy<Text>(ProcessText);
            }
            #endregion

            #region Geometry Colour Pass
            geometryColourFbo.Bind();
            ClearScreen();

            entityRenderer.Render(isMultisample: true, camera, CreateEntityRenderList());

            geometryColourFbo.Unbind();
            #endregion

            #region Geometry Data Pass
            geometryDataFbo.Bind();
            ClearScreen();
            
            skyboxRenderer.Render(isColourPass: false, camera);
            entityRenderer.Render(isMultisample: false, camera, CreateEntityRenderList());
            
            geometryDataFbo.Unbind();
            Game.Instance.SetGeometryDataFbo(geometryDataFbo);

            // Resolve colour to geometryDataFbo
            geometryColourFbo.ResolveToFbo(geometryDataFbo);
            #endregion
            
            #region Lighting Pass
            finalFbo.Bind();
            ClearScreen();
            
            skyboxRenderer.Render(isColourPass: true, camera);
            lightingRenderer.Render(geometryDataFbo);
            
            finalFbo.Unbind();
            #endregion
            
            #region Final Pass
            finalRenderer.Render(finalFbo);
            #endregion

            #region UI Pass
            SortingLayers.Sort();

            spriteRenderer.Start(camera);
            textRenderer.Start(camera);
            foreach (var sortingLayer in SortingLayers)
            {
                spriteRenderer.Render(CreateSpriteRenderList(sortingLayer));
                textRenderer.Render(CreateTextRenderList(sortingLayer));
            }
            spriteRenderer.Stop();
            textRenderer.Stop();
            #endregion

            #region Clear processed gameobjects
            entities.Clear();
            sprites.Clear();
            texts.Clear();
            #endregion
        }

        public void Dispose()
        {
            entityRenderer.Dispose();
            spriteRenderer.Dispose();
            textRenderer.Dispose();
            skyboxRenderer.Dispose();
            lightingRenderer.Dispose();
            finalRenderer.Dispose();

            geometryColourFbo.Dispose();
            finalFbo.Dispose();
            geometryDataFbo.Dispose();
        }

        private void SetupFrameBuffers()
        {
            int width = Game.Instance.Width;
            int height = Game.Instance.Height;

            #region Geometry Colour FrameBuffer
            geometryColourFbo = new FrameBuffer();

            geometryColourFbo.Width = width;
            geometryColourFbo.Height = height;
            geometryColourFbo.ColourAttachments = new List<ColourAttachment>()
            {
                new ColourAttachment(), // Colour Buffer
            };
            geometryColourFbo.DepthAttachment = new DepthAttachment();

            geometryColourFbo.Create();
            #endregion

            #region Geometry Data FrameBuffer
            geometryDataFbo = new FrameBuffer();

            geometryDataFbo.Width = width;
            geometryDataFbo.Height = height;
            geometryDataFbo.ColourAttachments = new List<ColourAttachment>()
            {
                new ColourAttachment(), // Colour Buffer
                new ColourAttachment(), // Normal Buffer
                new ColourAttachment(), // Position Buffer
                new ColourAttachment(), // Model Buffer
                new ColourAttachment(PixelInternalFormat.Rgba32i, PixelFormat.RedInteger, PixelType.Int), // InstanceId Buffer
            };
            geometryDataFbo.DepthAttachment = new DepthAttachment();

            geometryDataFbo.Create();
            #endregion

            #region Final FrameBuffer
            finalFbo = new FrameBuffer();
            
            finalFbo.Width = width;
            finalFbo.Height = height;
            finalFbo.ColourAttachments = new List<ColourAttachment>()
            {
                new ColourAttachment(), // Colour Buffer
            };

            finalFbo.DepthAttachment = new DepthAttachment();
            
            finalFbo.Create();
            #endregion
        }

        private void ProcessEntity(Entity entity)
        {
            entities.TryGetValue(entity.model.rawModel.Name, out List<Entity> batch);
            if (batch!= null)
            {
                batch.Add(entity);
            }
            else
            {
                List<Entity> newBatch = new();
                newBatch.Add(entity);
                entities[entity.model.rawModel.Name] = newBatch;
            }
        }
        private void ProcessSprite(Sprite sprite)
        {
            sprites.TryGetValue(sprite.texture, out List<Sprite> batch);
            if (batch != null)
            {
                batch.Add(sprite);
            }
            else
            {
                List<Sprite> newBatch = new();
                newBatch.Add(sprite);
                sprites[sprite.texture] = newBatch;
            }

            SortingLayers.UniqueAdd(sprite.SortingLayer);
        }
        private void ProcessText(Text text)
        {
            texts.TryGetValue(text.font, out List<Text> batch);
            if (batch != null)
            {
                batch.Add(text);
            }
            else
            {
                List<Text> newBatch = new();
                newBatch.Add(text);
                texts[text.font] = newBatch;
            }

            SortingLayers.UniqueAdd(text.SortingLayer);
        }

        private Dictionary<TexturedModel, List<Entity>> CreateEntityRenderList()
        {
            var output = new Dictionary<TexturedModel, List<Entity>>();
            foreach (string modelName in entities.Keys)
            {
                var modelNameOuput = new Dictionary<TexturedModel, List<Entity>>();
                entities.TryGetValue(modelName, out List<Entity> batch);

                foreach (var entity in batch)
                {
                    var found = false;
                    foreach (var model in modelNameOuput.Keys)
                    {
                        if (model.Equals(entity.model))
                        {
                            modelNameOuput[model].Add(entity);
                            found = true;
                            break;
                        }
                    }       
                    if (!found)
                    {
                        var newEntry = new List<Entity>
                        {
                            entity
                        };
                        modelNameOuput.Add(entity.model, newEntry);
                    }
                }
                foreach (var key in modelNameOuput.Keys)
                {
                    output.Add(key, modelNameOuput[key]);
                }
            }
            return output;
        }
        private Dictionary<GameFont, List<Text>> CreateTextRenderList(int sortingLayer)
        {
            var output = new Dictionary<GameFont, List<Text>>();
            foreach (var font in texts.Keys)
            {
                texts.TryGetValue(font, out List<Text> batch);
                var newBatch = new List<Text>();
                foreach (var text in batch)
                {
                    if (text.SortingLayer == sortingLayer)
                    {
                        newBatch.Add(text);
                    }
                }
                output.Add(font, newBatch);
            }
            return output;
        }
        private Dictionary<Texture, List<Sprite>> CreateSpriteRenderList(int sortingLayer)
        {
            var output = new Dictionary<Texture, List<Sprite>>();
            foreach (var texture in sprites.Keys)
            {
                sprites.TryGetValue(texture, out List<Sprite> batch);
                var newBatch = new List<Sprite>();
                foreach (var sprite in batch)
                {
                    if (sprite.SortingLayer == sortingLayer)
                    {
                        newBatch.Add(sprite);
                    }
                }
                output.Add(texture, newBatch);
            }
            return output;
        }

        private FrameBuffer geometryColourFbo;
        private FrameBuffer geometryDataFbo;
        private FrameBuffer finalFbo;

        private readonly Dictionary<string, List<Entity>> entities = new();
        private readonly Dictionary<Texture, List<Sprite>> sprites = new();
        private readonly Dictionary<GameFont, List<Text>> texts = new();

        private readonly List<int> SortingLayers = new();
    }
}
