using System;
using System.Drawing;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using Senapp.Engine.Entities;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Renderer;
using Senapp.Engine.Events;
using Senapp.Engine.Physics;
using Senapp.Engine.Raycasts;
using Senapp.Engine.Controllers;
using Senapp.Engine.Utilities.Testing;
using Senapp.Engine.Core.Scenes;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Loaders;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.Networking.Client;
using Senapp.Engine.Renderer.FrameBuffers;

namespace Senapp.Engine.Core
{
    public abstract class Game : GameWindow
    {
        public const int WINDOW_BORDER_SIZE = 9;

        public static Game Instance { get; private set; }

        public MasterRenderer Renderer { get; private set; }
        public RaycastManager RaycastManager { get; private set; }
        public PhysicsManager PhysicsManager { get; private set; }
        public SceneManager SceneManager { get; private set; }

        public float AspectRatio { get; private set; }

        public Scene MainScene;
        public Light SunLight;
        public Camera MainCamera;

        public List<GameObject> GetSceneGameObjects()
        {
            var total = new List<GameObject>();
            foreach (var scene in SceneManager.scenes.Values)
            {
                total.AddRange(scene.GetGameObjects());
            }
            return total;
        }
        public List<GameObject> GetAllGameObjects()
        {
            var total = new List<GameObject>();
            foreach (var scene in SceneManager.scenes.Values)
            {
                total.AddRange(scene.GetAllGameObjects());
            }
            return total;
        }
        public List<T> GetAllComponents<T>(bool includeDisabled = false) where T : Component, new()
        {
            var total = new List<T>();
            foreach (var scene in SceneManager.scenes.Values)
            {
                total.AddRange(scene.GetAllComponents<T>(includeDisabled));
            }
            return total;
        }
        public void TriggerIsGameObjectUpdate()
        {
            foreach (var gameObject in GetSceneGameObjects())
            {
                gameObject.transform.Translate(0);
            }
        }

        public void SetGeometryDataFbo(FrameBuffer fbo)
        {
            lastGeometryFbo = fbo;
        }

        protected Game (int width, int height, GraphicsMode graphicsMode, string title) : base(width, height, graphicsMode, title, GameWindowFlags.Default, DisplayDevice.Default, 4, 5, GraphicsContextFlags.ForwardCompatible)
        {
            if (Instance != null)
            {
                Console.WriteLine("[ENGINE] You should not have more then one Game class");
            }

            Instance = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            EssentialSetup();

            Loader.Initialize();

            OnGameInitialized();
        }
        protected override void OnClosed(EventArgs e)
        {
            Loader.Dispose();
            Renderer.Dispose();

            OnGameClosed();
            SceneManager.Dispose();

            Dispose();
        }
        protected override void OnResize(EventArgs e)
        {
            int screenHeight = ClientSize.Height;
            int screenWidth = ClientSize.Width;
            if (screenHeight <= 0 || screenWidth <= 0)
                return;

            Renderer.OnScreenResize(screenWidth, screenHeight);

            AspectRatio = Width / (float)Height;
            MainCamera.AspectRatio = AspectRatio;

            TriggerIsGameObjectUpdate();

            OnResize();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            FrameRate.Update();

            MasterRenderer.ClearScreen();
            Renderer.Render(SunLight, MainCamera);

            var args = new GameRenderedEventArgs((float)e.Time);
            OnGameRendered(args);

            SwapBuffers();
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

            RaycastUpdate(args);
            PhysicsUpdate(args);
            NetworkUpdate(args);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            lastKnowMousePositionOnWindow = new Vector2(e.X, e.Y);
            RaycastUpdate(null, true);
            RaycastManager.RaycastDown();
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            lastKnowMousePositionOnWindow = new Vector2(e.X, e.Y);
            RaycastUpdate(null, true);
            RaycastManager.RaycastUp();
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            lastKnowMousePositionOnWindow = new Vector2(e.X, e.Y);
        }

        protected event GameInitializedEventHandler GameInitializedEvent;
        protected event GameResizeEventHandler GameResizeEvent;
        protected event GameUpdatedEventHandler GameUpdatedEvent;
        protected event GameRenderedEventHandler GameRenderedEvent;
        protected event GameClosedEventHandler GameClosedEvent;

        private void EssentialSetup()
        {
            SunLight = new GameObject()
                .WithName("Sun Light")
                .WithPosition(new Vector3(100, 100, 100))
                .WithColour(Color.White)
                .WithComponent(new Entity(Geometries.Sphere)
                    .WithLuminosity(1))
                .AddComponent(new Light());

            MainCamera = new GameObject()
                .WithName("Main Camera")
                .WithComponent(new Camera(Width / (float)Height, 60))
                .WithComponent(new CameraController())
                .GetComponent<Camera>();

            MainScene = new Scene()
                .WithGameObject(SunLight.gameObject)
                .WithGameObject(MainCamera.gameObject);

            Renderer = new MasterRenderer(MainCamera);
            RaycastManager = new RaycastManager(MainCamera);
            PhysicsManager = new PhysicsManager();
            SceneManager = new SceneManager();

            SceneManager.AddScene(MainScene);
        }

        private void RaycastUpdate(GameUpdatedEventArgs args, bool force = false)
        {
            if (force || raycastUpdateDeltaTime + args.DeltaTime > raycastUpdateFrequency)
            {
                raycastUpdateDeltaTime = 0;
                if (Input.IsMouseOnWindow(lastKnowMousePositionOnWindow))
                {
                    RaycastManager.RaycastUISendingUpdate(lastKnowMousePositionOnWindow);

                    lastInstanceId = lastGeometryFbo.ReadPixel(4, new Vector2(lastKnowMousePositionOnWindow.X, Height - lastKnowMousePositionOnWindow.Y));
                    RaycastManager.InstanceIdUpdate(lastInstanceId);
                }
            }
            else
            {
                raycastUpdateDeltaTime += args.DeltaTime;
            }
        } 
        private void PhysicsUpdate(GameUpdatedEventArgs args, bool force = false)
        {
            if (force || physicsUpdateDeltaTime + args.DeltaTime > physicsUpdateFrequency)
            {
                physicsUpdateDeltaTime = 0;
                PhysicsManager.PhysicsUpdate(args);
            }
            else
            {
                physicsUpdateDeltaTime += args.DeltaTime;
            }
        }
        private void NetworkUpdate(GameUpdatedEventArgs args, bool force = false)
        {
            if (force || networkUpdateDeltaTime + args.DeltaTime > networkUpdateFrequency)
            {
                networkUpdateDeltaTime = 0;
                _ = NetworkClient.Update();
            }
            else
            {
                networkUpdateDeltaTime += args.DeltaTime;
            }
        }

        private void OnGameInitialized() { GameInitializedEvent?.Invoke(this); }
        private void OnGameUpdated(GameUpdatedEventArgs args) { GameUpdatedEvent?.Invoke(this, args); }
        private void OnGameRendered(GameRenderedEventArgs args) { GameRenderedEvent?.Invoke(this, args); }
        private void OnGameClosed() { GameClosedEvent?.Invoke(this); }
        private void OnResize() { GameResizeEvent?.Invoke(this); }

        private FrameBuffer lastGeometryFbo;
        private Vector2 lastKnowMousePositionOnWindow;
        private int lastInstanceId;

        private const float raycastUpdateFrequency = 0.01666666666f; // 60 updates per second
        private const float physicsUpdateFrequency = 0.01666666666f; // 60 updates per second
        private const float networkUpdateFrequency = 0.33333333333f; // 3 updates per second

        private float raycastUpdateDeltaTime = 0;
        private float physicsUpdateDeltaTime = 0;
        private float networkUpdateDeltaTime = 0;
    }
}
