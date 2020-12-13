using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK;

using Senapp.Engine;
using Senapp.Engine.Renderer;
using Senapp.Engine.Events;
using Senapp.Engine.Models;
using Senapp.Engine.Entities;
using Senapp.Engine.Base;
using Senapp.Engine.Terrains;
using System.Collections.Generic;
using Senapp.Engine.UI;
using System.Diagnostics;
using ImGuiNET;
using Senapp.Engine.ImGUI;
using OpenTK.Graphics;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Senapp
{
    public class TestGame : Game
    {
        public TestGame(GraphicsMode gMode) : base(WIDTH, HEIGHT, gMode, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameResizeEvent += Resized;
            GameUpdatedEvent += Update;
            GameRenderedEvent += Render;
            GameClosedEvent += Close;
            Run();
        }

        public static readonly bool START_FULLSCREEN = false;
        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "Test";

        public GameFont font = new GameFont();

        private void Initialize(object sender)
        {
            font.LoadFont("arial");
            WindowInitialization();
            GameInitilaztion();
        }
        private void WindowInitialization()
        {
            VSync = VSyncMode.On;
            if (START_FULLSCREEN)
                WindowState = WindowState.Fullscreen;
        }
        private void GameInitilaztion()
        {  
            //GameObject plane = new GameObject();
            //int size = 25;
            //plane.AddComponent(new Entity(Terrain.GenerateTerrain(size),""));
            //plane.isStatic = true;
            //plane.transform = new Transform(size / 2, -1.2f, size / 2); 

            GameObject earth = new GameObject();
            earth.AddComponent(new Entity(Geometries.Sphere, "earth8k"));
            earth.transform = new Transform(0, 0, 3);

            GameObject mars = new GameObject();
            mars.AddComponent(new Entity(Geometries.Sphere, "mars"));
            mars.transform = new Transform(0, 0, -3);

            GameObject pyramid = new GameObject();
            pyramid.AddComponent(new Entity(Geometries.Pyramid, "pyramid"));
            pyramid.transform = new Transform(-3, 0, 0);

            GameObject cube = new GameObject();
            cube.AddComponent(new Entity(Geometries.Cube, ""));
            cube.transform = new Transform(3, 0, 0);


            sunLight.transform = new Transform(0, 2, 0, 0, 0, 0, .2f, .2f, .2f);
            sunLight.AddComponent(new Entity(Geometries.Sphere));
            sunLight.GetComponent<Entity>().model.luminosity = 1;
            mainCamera.transform = new Transform(3.754f, 3.5f, 4.893f);
            var cam = mainCamera.GetComponent<Camera>();
            cam.Yaw = 235f;
            cam.Pitch = -30f;

            text1.AddComponent(new Text("FPS", font, new Sprite(font.fontAtlas), 10, UIConstraint.TopLeft));
            text1.transform = new Transform(0, 0, 0);
            text3.AddComponent(new Text("Memory", font, new Sprite(font.fontAtlas), 10, UIConstraint.TopLeft));
            text3.transform = new Transform(0, 75, 0); 
        }
        private GameObject text1 = new GameObject();
        private GameObject text3 = new GameObject();

        private bool CameraFollowMouse;


        private void DebugScreenTexts()
        {
            text1.GetComponent<Text>().UpdateText("FPS: " + GetFrameRate().ToString());
            text3.GetComponent<Text>().UpdateText(string.Format("Memory: {0}mb", MathF.Round(Process.GetCurrentProcess().PrivateMemorySize64 / 1024f / 1024f)));
        }
        private void Update(object sender, GameUpdatedEventArgs args)
        {
            foreach (var gameObject in GameObject.GameObjects)
            {
                if (gameObject.HasComponent<Entity>() && !gameObject.HasComponent<Light>() && !gameObject.isStatic) gameObject.transform.Rotate(0, 0, 0.01f);
            }
            if (!Focused)
                return;
            if (Input.GetKeyDown(Key.E))
                EditorWindow.enabled = !EditorWindow.enabled;
            if (EditorWindow.enabled) return;

            if (Input.GetKeyDown(Key.Escape))
                Exit();

            if (Input.GetKeyDown(Key.F))
                FrameRateCapture(!GetFrameRateEnabled());
            if (Input.GetKeyDown(Key.V))
            {
                if (VSync == VSyncMode.Off)
                    VSync = VSyncMode.On;
                else 
                    VSync = VSyncMode.Off;
            }
            if (Input.GetKeyDown(Key.L))
            {
                Input.LockCursor(!Input.GetLockCursor());
                Input.ShowCursor(!Input.GetCursorVisibility());
                CameraFollowMouse = !CameraFollowMouse;
                if (Input.GetLockCursor())
                    Input.SetMousePositionScreenCenter(0, 0);
            }

            const float cameraSpeed = 10.0f;
            var camera = mainCamera.GetComponent<Camera>();
            if (Input.GetKey(Key.W))
                mainCamera.transform.position += camera.Front * cameraSpeed * args.DeltaTime; 
            if (Input.GetKey(Key.S))
                mainCamera.transform.position -= camera.Front * cameraSpeed * args.DeltaTime; 
            if (Input.GetKey(Key.A))
                mainCamera.transform.position -= camera.Right * cameraSpeed * args.DeltaTime; 
            if (Input.GetKey(Key.D))
                mainCamera.transform.position += camera.Right * cameraSpeed * args.DeltaTime;
            if (Input.GetKey(Key.W))
                mainCamera.transform.position += camera.Front * cameraSpeed * args.DeltaTime;
            if (Input.GetKey(Key.S))
                mainCamera.transform.position -= camera.Front * cameraSpeed * args.DeltaTime;

            if (CameraFollowMouse)
            {
                Vector2 delta = Input.GetMouseDelta();
                if (Input.GetLockCursor())
                    Input.SetMousePositionScreenCenter(0, 0);
                camera.Pitch -= delta.Y * camera.Sensitivity ;
                camera.Yaw += delta.X * camera.Sensitivity;
            }            
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            mainCamera.GetComponent<Camera>().Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }

        private void Render(object sender, GameRenderedEventArgs e)
        {
            DebugScreenTexts();
        }
        private void Resized(object sender) { }
        private void Close(object sender) { }
    }
}
