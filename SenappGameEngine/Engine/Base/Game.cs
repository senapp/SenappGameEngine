using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.Utilities;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Renderer;
using Senapp.Engine.Events;
using Senapp.Engine.Physics;

namespace Senapp.Engine.Base
{
    public abstract class Game : GameWindow
    {
        public static Game Instance { get; private set; }
        public static List<GameObject> GameObjects = new List<GameObject>();

        public static readonly int WINDOW_BORDER_SIZE = 9;
        public float AspectRatio;

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

        protected Game (int width, int height, GraphicsMode gMode, string title) : base(width, height, gMode, title, GameWindowFlags.Default, DisplayDevice.Default, 4, 5, GraphicsContextFlags.ForwardCompatible)
        {
            if (Instance != null) Console.WriteLine("You should not have more then one Game class");
            Instance = this;

            //TargetRenderFrequency = 60;
            //TargetUpdateFrequency = 30;
        }

        private MasterRenderer renderer = new MasterRenderer();
        private RaycastManager raycastManager = new RaycastManager();
        private PhysicsManager physicsManager = new PhysicsManager();

        public static GameObject sunLight = new GameObject();
        public static GameObject mainCamera = new GameObject();

        protected override void OnClosed(EventArgs e)
        {
            renderer.Dispose();
            Loader.Dispose();
            OnGameClosed();
            Dispose();
        }
        protected override void OnLoad(EventArgs e)
        {
            mainCamera.AddComponent(new Camera(Width / (float)Height, 60));
            sunLight.AddComponent(new Light());
            sunLight.SetColour(Vector3.One);

            MasterRenderer.Initialize();
            renderer.Initiliaze(mainCamera.GetComponent<Camera>());

            GL.Enable(EnableCap.Texture2D);
            GL.Ortho(0, Width, Height, 0, 0, 1);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            OnGameInitialized();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var time = new GameUpdatedEventArgs((float)e.Time);
            Input.Update();
            ControllerManager.Update();
            OnGameUpdated(time);

            foreach (var gameObject in GameObjects)
            {
                if (!gameObject.enabled) continue;
                foreach (var component in gameObject.componentManager.GetComponents())
                {
                    component.Value.Update(time);
                } 
            }
            physicsManager.PhysicsUpdate(time);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            raycastManager.RaycastUISendingUpdate(e);
            raycastManager.RaycastSendingUpdate(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            raycastManager.RaycastClickCheck();
        }
        protected override void OnResize(EventArgs e)
        {
            int screenHeight = ClientSize.Height;
            int screenWidth = ClientSize.Width;
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
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            FrameRate.Update();
            MasterRenderer.ClearScreen();

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.Blend);

            renderer.Render(sunLight.GetComponent<Light>(), mainCamera.GetComponent<Camera>());
            OnGameRendered(new GameRenderedEventArgs((float)e.Time));

            SwapBuffers();
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }
    }
}
