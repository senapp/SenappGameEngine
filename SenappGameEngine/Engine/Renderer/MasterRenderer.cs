using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.ComponentRenderers;
using Senapp.Engine.Renderer.PostProcessing;
using Senapp.Engine.Shaders;
using Senapp.Engine.Shaders.Components;
using Senapp.Engine.Terrains;
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

        public MasterRenderer(Camera camera)
        {
            GL.Ortho(0, Game.Instance.Width, Game.Instance.Height, 0, 0, 1);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);

            EnableCulling();

            entityShader = new EntityShader();
            terrainShader = new TerrainShader();
            spriteShader = new SpriteShader();
            textShader = new TextShader();
            skyboxShader = new SkyboxShader();

            entityRenderer = new EntityRenderer(entityShader);
            terrainRenderer = new TerrainRenderer(terrainShader);
            spriteRenderer = new SpriteRenderer(spriteShader);
            textRenderer = new TextRenderer(textShader);
            skyboxRenderer = new SkyboxRenderer(skyboxShader, camera.GetProjectionMatrix());

            int width = Game.Instance.Width;
            int height = Game.Instance.Height;

            postProcessingMultisampledFbo = new Fbo(width, height);
            postProcessingOutputFbo = new Fbo(width, height, DepthBufferType.DEPTH_TEXTURE);
            postProcessingManager = new PostProcessingManager();
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
            postProcessingMultisampledFbo?.Dispose();
            postProcessingOutputFbo?.Dispose();
            postProcessingMultisampledFbo = new Fbo(width, height);
            postProcessingOutputFbo = new Fbo(width, height, DepthBufferType.DEPTH_TEXTURE);

            postProcessingManager.OnResize(width, height);
        }

        public void Render(Light sun, Camera camera)
        {
            foreach (var gameObject in Game.Instance.GetSceneGameObjects())
            {
                gameObject.ProccessRenderHierarchy<Entity>(ProcessEntity);
                gameObject.ProccessRenderHierarchy<Terrain>(ProcessTerrain);
                gameObject.ProccessRenderHierarchy<Sprite>(ProcessSprite);
                gameObject.ProccessRenderHierarchy<Text>(ProcessText);
            }

            if (GraphicsSettings.PostProcessingRequired)
            {
                postProcessingMultisampledFbo.Bind();
                ClearScreen();
            }
            #region 3D / World
            skyboxRenderer.Render(camera);

            entityShader.Start();
            entityShader.UpdateCamera(camera);
            entityRenderer.Render(CreateEntityRenderList());
            entityShader.LoadLight(sun);
            entityShader.Stop();

            terrainShader.Start();
            terrainShader.UpdateCamera(camera);
            terrainRenderer.Render(terrains);
            terrainShader.LoadLight(sun);
            terrainShader.Stop();
            #endregion
            if (GraphicsSettings.PostProcessingRequired)
            {
                postProcessingMultisampledFbo.Unbind();
                postProcessingMultisampledFbo.ResolveToFbo(postProcessingOutputFbo);
                postProcessingManager.ApplyPostProcessing(postProcessingOutputFbo.ColourTexture);
            }

            #region 2D / UI
            SortingLayers.Sort();

            spriteShader.Start();
            spriteShader.UpdateCamera(camera);
            textShader.Start();
            textShader.UpdateCamera(camera);
            foreach (var sortingLayer in SortingLayers)
            {
                spriteRenderer.Render(CreateSpriteRenderList(sortingLayer));
                textRenderer.Render(CreateTextRenderList(sortingLayer));
            }
            spriteShader.Stop();
            textShader.Stop();
            #endregion

            terrains.Clear();
            entities.Clear();
            sprites.Clear();
            texts.Clear();
        }

        public void Dispose()
        {
            entityShader.Dispose();
            terrainShader.Dispose();
            spriteShader.Dispose();
            textShader.Dispose();
            skyboxShader.Dispose();

            skyboxRenderer.Dispose();

            postProcessingMultisampledFbo.Dispose();
            postProcessingOutputFbo.Dispose();
            postProcessingManager.Dispose();
        }

        private void ProcessTerrain(Terrain terrain)
        {
            terrains.TryGetValue(terrain, out List<Terrain> batch);
            if (batch != null)
            {
                batch.Add(terrain);
            }
            else
            {
                List<Terrain> newBatch = new();
                newBatch.Add(terrain);
                terrains[terrain] = newBatch;
            }
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

        private EntityShader entityShader;
        private TerrainShader terrainShader;
        private SpriteShader spriteShader;
        private TextShader textShader;
        private SkyboxShader skyboxShader;

        private EntityRenderer entityRenderer;
        private TerrainRenderer terrainRenderer;
        private SpriteRenderer spriteRenderer;
        private TextRenderer textRenderer;
        private SkyboxRenderer skyboxRenderer;

        private PostProcessingManager postProcessingManager;
        private Fbo postProcessingMultisampledFbo;
        private Fbo postProcessingOutputFbo;

        private readonly Dictionary<string, List<Entity>> entities = new();
        private readonly Dictionary<Terrain, List<Terrain>> terrains = new();
        private readonly Dictionary<Texture, List<Sprite>> sprites = new();
        private readonly Dictionary<GameFont, List<Text>> texts = new();

        private readonly List<int> SortingLayers = new();
    }
}
