using System;
using OpenTK.Graphics.OpenGL;
using Senapp.Engine.Renderer;
using Senapp.Engine.Events;
using OpenTK;
using OpenTK.Graphics;
using Senapp.Engine.Base;
using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.ImGUI;

namespace Senapp.Engine
{
    public abstract class Game : GameWindow
    {
        public static Game Instance { get; private set; }
        public static readonly int WINDOW_BORDER_SIZE = 9;
        public static float AspectRatio;

        protected event GameInitializedEventHandler GameInitializedEvent;
        private void OnGameInitialized() { GameInitializedEvent?.Invoke(this); }

        protected event GameUpdatedEventHandler GameUpdatedEvent;
        private void OnGameUpdated(GameUpdatedEventArgs args) { GameUpdatedEvent?.Invoke(this, args); }

        protected event GameRenderedEventHandler GameRenderedEvent;
        private void OnGameRendered(GameRenderedEventArgs args) { GameRenderedEvent?.Invoke(this, args); }

        protected event GameClosedEventHandler GameClosedEvent;
        private void OnGameClosed() { GameClosedEvent?.Invoke(this); }

        protected event GameResizeEventHandler GameResizeEvent;
        private void OnResize() { GameResizeEvent?.Invoke(this); }

        protected Game (int width, int height, GraphicsMode gMode, string 
            title) : base(width, height, gMode, title,
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4, 5, GraphicsContextFlags.ForwardCompatible)
        {
            if (Instance != null)
                Console.WriteLine("You should not have more then one Game class");
            Instance = this;
        }
        private void CreateDummy()
        {
            GameObject g = new GameObject();
            g.excludeFromEditor = true;
            g.AddComponent(new Entity(Geometries.Cube));
            g.transform = new Transform(0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
        protected override void OnClosed(EventArgs e)
        {
            renderer.Dispose();
            Loader.Dispose();
            OnGameClosed();
            Dispose();
        }
        protected override void OnLoad(EventArgs e)
        {
            MasterRenderer.Initialize();
            renderer.Initiliaze();
            CreateDummy();
            mainCamera.AddComponent(new Camera(Width / (float)Height, 60));
            mainCamera.transform = new Transform(0, 0, 5);
            sunLight.AddComponent(new Light(Vector3.One));
            sunLight.transform = new Transform(200, 200, 100);
            GL.Enable(EnableCap.Texture2D);
            GL.Ortho(0, Width, Height, 0, 0, 1);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            EditorWindow.Init(this);
            OnGameInitialized();
        }

        private int lastTick;
        private int lastFrameRate;
        private int frameRate;
        private bool frameRateCaptureEnabled = true;

        public void FrameRateCapture(bool boolean)
        {
            frameRateCaptureEnabled = boolean;
        }
        public bool GetFrameRateEnabled()
        {
            return frameRateCaptureEnabled;
        }
        public int GetFrameRate()
        {
            return lastFrameRate;
        }

        private MasterRenderer renderer = new MasterRenderer();
        public GameObject sunLight = new GameObject();
        public GameObject mainCamera = new GameObject();

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (frameRateCaptureEnabled)
            {
                if (Environment.TickCount - lastTick >= 1000)
                {
                    lastFrameRate = frameRate;
                    frameRate = 0;
                    lastTick = System.Environment.TickCount;
                }
                frameRate++;
            }
            foreach (var gameObject in GameObject.GameObjects)
            {
                foreach (var component in gameObject.componentManager.GetComponents())
                {
                    component.Value.Update();
                }
            }
            Input.Update();
            OnGameUpdated(new GameUpdatedEventArgs((float)e.Time));
        }
        protected override void OnResize(EventArgs e)
        {
            int screenHeight = ClientSize.Height;
            int screenWidth = ClientSize.Width;
            Console.WriteLine(screenHeight + " " + screenWidth);
            if (screenHeight <= 0 || screenWidth <= 0)
                return;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, screenWidth, screenHeight, 0.0, 1.0, -1.0);
            var ratioX = Width / (float)screenWidth;
            var ratioY = Height / (float)screenHeight;
            var ratio = ratioX < ratioY ? ratioX : ratioY;
            var viewWidth = Convert.ToInt32(screenWidth * ratio);
            var viewHeight = Convert.ToInt32(screenHeight * ratio);
            var viewX = Convert.ToInt32((Width - screenWidth * ratio) / 2);
            var viewY = Convert.ToInt32((Height - screenHeight * ratio) / 2);
            GL.Viewport(viewX, viewY, viewWidth, viewHeight);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            AspectRatio = Width / (float)Height;
            mainCamera.GetComponent<Camera>().AspectRatio = AspectRatio;
            OnResize();
            EditorWindow.OnResize(this);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            MasterRenderer.ClearScreen();
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            renderer.Render(sunLight, mainCamera);
            OnGameRendered(new GameRenderedEventArgs((float)e.Time));
            EditorWindow.Render(this, new GameRenderedEventArgs((float)e.Time));
            SwapBuffers();
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            EditorWindow.OnKeyPress(e);
        }
    }
}
