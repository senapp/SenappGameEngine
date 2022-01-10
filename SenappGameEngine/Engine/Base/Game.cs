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
using System.Linq;

namespace Senapp.Engine.Base
{
    public abstract class Game : GameWindow
    {
        public static Game Instance { get; private set; }

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
        }

        public MasterRenderer Renderer;
        public RaycastManager RaycastManager;
        public PhysicsManager PhysicsManager;
        public SceneManager SceneManager;

        public Scene MainScene;
        public Light SunLight;
        public Camera MainCamera;

        /// <summary>
        /// Gets scene game objects. 
        /// </summary>
        /// <returns>All GameObjects at the root of each scene, children of these GameObjects should be gotten by calling GetChildren() on the gameObject</returns>
        public static List<GameObject> GetSceneGameObjects()
        {
            var total = new List<GameObject>();
            foreach (var scene in Instance.SceneManager.scenes.Values)
            {
                total.AddRange(scene.GetGameObjects());
            }
            return total;
        }

        /// <summary>
        /// Gets all GameObjects
        /// </summary>
        /// <returns>All GameObjects currently being used.</returns>
        public static List<GameObject> GetAllGameObjects()
        {
            var total = new List<GameObject>();
            foreach (var scene in Instance.SceneManager.scenes.Values)
            {
                total.AddRange(scene.GetAllGameObjects());
            }
            return total;
        }

        private void EssentialSetup()
        {
            SunLight = new GameObject()
                .WithName("Sun Light")
                .WithColour(Vector3.One)
                .AddComponent(new Light());

            MainCamera = new GameObject()
                .WithName("Main Camera")
                .AddComponent(new Camera(Width / (float)Height, 60));

            MainScene = new Scene()
                .WithGameObject(SunLight.gameObject)
                .WithGameObject(MainCamera.gameObject);

            Renderer = new MasterRenderer();
            RaycastManager = new RaycastManager(MainCamera);
            PhysicsManager = new PhysicsManager();
            SceneManager = new SceneManager();

            SceneManager.AddScene(MainScene);
        }

        protected override void OnClosed(EventArgs e)
        {
            Renderer.Dispose();
            Loader.Dispose();
            OnGameClosed();
            SceneManager.Dispose();
            Dispose();
        }
        protected override void OnLoad(EventArgs e)
        {
            EssentialSetup();

            MasterRenderer.Initialize();
            Renderer.Initiliaze(MainCamera);

            GL.Enable(EnableCap.Texture2D);
            GL.Ortho(0, Width, Height, 0, 0, 1);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            OnGameInitialized();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var args = new GameUpdatedEventArgs((float)e.Time);

            Input.Update();
            ControllerManager.Update();

            OnGameUpdated(args);

            foreach (var gameObject in GetSceneGameObjects())
            {
                gameObject.Update(args);
            }

            PhysicsManager.PhysicsUpdate(args);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            RaycastManager.RaycastUISendingUpdate(e);
            RaycastManager.RaycastSendingUpdate(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            RaycastManager.RaycastClickCheck();
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
            MainCamera.AspectRatio = AspectRatio;
            OnResize();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            FrameRate.Update();
            MasterRenderer.ClearScreen();

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.Blend);

            Renderer.Render(SunLight, MainCamera);
            OnGameRendered(new GameRenderedEventArgs((float)e.Time));

            SwapBuffers();
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }
    }
}
