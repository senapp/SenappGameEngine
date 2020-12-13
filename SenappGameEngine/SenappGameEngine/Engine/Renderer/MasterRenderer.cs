using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;
using Senapp.Engine.Terrains;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using Senapp.Engine.UI;
using OpenTK;
using Senapp.Engine.Base;

namespace Senapp.Engine.Renderer
{
    public class MasterRenderer
    {
        private EntityShader entityShader;
        private TerrainShader terrainShader;
        private UIShader shaderUI;
        private TextShader textShader;

        private EntityRenderer entityRenderer;
        private TerrainRenderer terrainRenderer;
        private UIRenderer rendererUI;
        private TextRenderer textRenderer;


        private Dictionary<TexturedModel, List<GameObject>> entities = new Dictionary<TexturedModel, List<GameObject>>();
        private List<GameObject> terrains = new List<GameObject>();
        private Dictionary<Texture, List<GameObject>> UIElements = new Dictionary<Texture, List<GameObject>>();
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
        public void Initiliaze()
        {
            entityShader = new EntityShader();
            terrainShader = new TerrainShader();
            shaderUI = new UIShader();
            textShader = new TextShader();

            entityRenderer = new EntityRenderer(entityShader);
            terrainRenderer = new TerrainRenderer(terrainShader);
            rendererUI = new UIRenderer(shaderUI);
            textRenderer = new TextRenderer(textShader);
        }
        public void Render(GameObject sun, GameObject camera)
        {
            foreach (var gameObject in GameObject.GameObjects)
            {
                if (gameObject.HasComponent<Entity>() && gameObject.enabled) ProcessEntity(gameObject);
                else if (gameObject.HasComponent<Terrain>() && gameObject.enabled) ProcessTerrain(gameObject);
                else if (gameObject.HasComponent<UIElement>() && gameObject.enabled) ProcessUIElement(gameObject);
                else if (gameObject.HasComponent<Text>() && gameObject.enabled) ProcessText(gameObject);

            }

            entityShader.Start();
            entityShader.UpdateCamera(camera.GetComponent<Camera>(), camera.transform);
            entityRenderer.Render(entities);
            entityShader.LoadLight(sun.GetComponent<Light>(), sun.transform);
            entityShader.Stop();

            terrainShader.Start();
            terrainShader.UpdateCamera(camera.GetComponent<Camera>(), camera.transform);
            terrainRenderer.Render(terrains);
            terrainShader.LoadLight(sun.GetComponent<Light>(), sun.transform);
            terrainShader.Stop();

            shaderUI.Start();
            shaderUI.UpdateCamera(camera.GetComponent<Camera>(), camera.transform);
            rendererUI.Render(UIElements, camera.transform.position);
            shaderUI.Stop();

            textShader.Start();
            textShader.UpdateCamera(camera.GetComponent<Camera>(), camera.transform);
            textRenderer.Render(texts, camera.transform.position);
            textShader.Stop();

            terrains.Clear();
            entities.Clear();
            UIElements.Clear();
            texts.Clear();
        }
        public void ProcessTerrain(GameObject terrain)
        {
            terrains.Add(terrain);
        }

        public void ProcessEntity(GameObject entity)
        {
            TexturedModel entityModel = entity.GetComponent<Entity>().model;
            entities.TryGetValue(entityModel, out List<GameObject> batch);
            if (batch!= null)
            {
                batch.Add(entity);
            }
            else
            {
                List<GameObject> newBatch = new List<GameObject>();
                newBatch.Add(entity);
                entities[entityModel] = newBatch;
            }
        }
        public void ProcessUIElement(GameObject element)
        {
            Texture texture = element.GetComponent<UIElement>().sprite.texture;
            UIElements.TryGetValue(texture, out List<GameObject> batch);
            if (batch != null)
            {
                batch.Add(element);
            }
            else
            {
                List<GameObject> newBatch = new List<GameObject>();
                newBatch.Add(element);
                UIElements[texture] = newBatch;
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
