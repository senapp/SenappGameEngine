using System.Collections.Generic;
using System.Drawing;

using OpenTK.Graphics.OpenGL4;

using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;
using Senapp.Engine.Terrains;
using Senapp.Engine.UI;
using Senapp.Engine.Base;

namespace Senapp.Engine.Renderer
{
    public class MasterRenderer
    {
        private EntityShader entityShader;
        private TerrainShader terrainShader;
        private SpriteShader shaderUI;
        private TextShader textShader;
        private SkyboxShader skyboxShader;

        private EntityRenderer entityRenderer;
        private TerrainRenderer terrainRenderer;
        private SpriteRenderer spriteRenderer;
        private TextRenderer textRenderer;
        private SkyboxRenderer skyboxRenderer;

        private Dictionary<string, List<GameObject>> entities = new Dictionary<string, List<GameObject>>();
        private List<GameObject> terrains = new List<GameObject>();
        private Dictionary<Texture, List<GameObject>> sprites = new Dictionary<Texture, List<GameObject>>();
        private Dictionary<GameFont, List<GameObject>> texts = new Dictionary<GameFont, List<GameObject>>();

        public static void Initialize(float red = 0.0f, float green = 0.0f, float blue = 0.0f, float alpha = 0.0f)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            EnableCulling();
        }
        public static void SetClearColour255(float red = 0, float green = 0, float blue = 0, float alpha = 0)
        {
            red = red / 255;
            green = green / 255;
            blue = blue / 255;
            alpha = alpha / 255;
            GL.ClearColor(red, green, blue, alpha);
        }
        public static void SetClearColour(float red = 0, float green = 0, float blue = 0, float alpha = 0)
        {
            GL.ClearColor(red, green, blue, alpha);
        }
        public static void SetClearColour(Color colour)
        {
            GL.ClearColor(colour);
        }
        public static void ClearScreen()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0, 0, 0, 1);
        }

        public void Initiliaze(Camera camera)
        {
            entityShader = new EntityShader();
            terrainShader = new TerrainShader();
            shaderUI = new SpriteShader();
            textShader = new TextShader();
            skyboxShader = new SkyboxShader();

            entityRenderer = new EntityRenderer(entityShader);
            terrainRenderer = new TerrainRenderer(terrainShader);
            spriteRenderer = new SpriteRenderer(shaderUI);
            textRenderer = new TextRenderer(textShader);
            skyboxRenderer = new SkyboxRenderer(skyboxShader, camera.GetProjectionMatrix());
        }
        public void Render(Light sun, Camera camera)
        {
            foreach (var gameObject in Game.GameObjects)
            {
                if (gameObject.HasComponent<Entity>() && gameObject.enabled) ProcessEntity(gameObject);
                else if (gameObject.HasComponent<Terrain>() && gameObject.enabled) ProcessTerrain(gameObject);
                else if (gameObject.HasComponent<Sprite>() && gameObject.enabled) ProcessSprite(gameObject);
                else if (gameObject.HasComponent<Text>() && gameObject.enabled) ProcessText(gameObject);
            }

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

            shaderUI.Start();
            shaderUI.UpdateCamera(camera);
            spriteRenderer.Render(sprites, camera.gameObject.transform.position);
            shaderUI.Stop();

            textShader.Start();
            textShader.UpdateCamera(camera);
            textRenderer.Render(texts, camera.gameObject.transform.position);
            textShader.Stop();

            terrains.Clear();
            entities.Clear();
            sprites.Clear();
            texts.Clear();
        }
        private bool TextureModelCompare(TexturedModel a, TexturedModel b)
        {
            var equal = (a.texture == b.texture &&
                a.shineDamper == b.shineDamper &&
                a.reflectivity == b.reflectivity &&
                a.luminosity == b.luminosity);

            return equal;
        }

        private Dictionary<TexturedModel, List<GameObject>> CreateEntityRenderList()
        {
            var output = new Dictionary<TexturedModel, List<GameObject>>();
            foreach (string modelName in entities.Keys)
            {
                var modelNameOuput = new Dictionary<TexturedModel, List<GameObject>>();
                entities.TryGetValue(modelName, out List<GameObject> batch);

                foreach (var gameObject in batch)
                {
                    var gameObjectModel = gameObject.GetComponent<Entity>().model;
                    var found = false;
                    foreach (var model in modelNameOuput.Keys)
                    {
                        if (TextureModelCompare(model, gameObjectModel))
                        {
                            modelNameOuput[model].Add(gameObject);
                            found = true;
                            break;
                        }
                    }       
                    if (!found)
                    {
                        var newEntry = new List<GameObject>();
                        newEntry.Add(gameObject);
                        modelNameOuput.Add(gameObjectModel, newEntry);
                    }
                }
                foreach (var key in modelNameOuput.Keys)
                {
                    output.Add(key, modelNameOuput[key]);
                }
            }
            return output;
        }
        public void ProcessTerrain(GameObject terrain)
        {
            terrains.Add(terrain);
        }
        public void ProcessEntity(GameObject entity)
        {
            TexturedModel entityModel = entity.GetComponent<Entity>().model;
            string rawModelName = entityModel.rawModel.rawModelName;
            entities.TryGetValue(rawModelName, out List<GameObject> batch);
            if (batch!= null)
            {
                batch.Add(entity);
            }
            else
            {
                List<GameObject> newBatch = new List<GameObject>();
                newBatch.Add(entity);
                entities[rawModelName] = newBatch;
            }
        }
        public void ProcessSprite(GameObject element)
        {
            Texture texture = element.GetComponent<Sprite>().texture;
            sprites.TryGetValue(texture, out List<GameObject> batch);
            if (batch != null)
            {
                batch.Add(element);
            }
            else
            {
                List<GameObject> newBatch = new List<GameObject>();
                newBatch.Add(element);
                sprites[texture] = newBatch;
            }
        }
        public void ProcessText(GameObject text)
        {
            GameFont font = text.GetComponent<Text>().Font;
            texts.TryGetValue(font, out List<GameObject> batch);
            if (batch != null)
            {
                batch.Add(text);
            }
            else
            {
                List<GameObject> newBatch = new List<GameObject>();
                newBatch.Add(text);
                texts[font] = newBatch;
            }
        }

        public void Dispose()
        {
            entityShader.CleanUp();
            terrainShader.CleanUp();
            shaderUI.CleanUp();
            textShader.CleanUp();
            skyboxShader.CleanUp();
        }

        public static void EnableCulling()
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }
        public static void DisableCulling()
        {
            GL.Disable(EnableCap.CullFace);
        }
    }
}
