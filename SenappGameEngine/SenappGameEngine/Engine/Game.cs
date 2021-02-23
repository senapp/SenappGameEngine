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
using Senapp.Engine.Utilities;
using Senapp.Engine.Physics;
using System.Threading;
using OpenTK.Input;
using System.Drawing;
using System.Collections.Generic;
using Senapp.Engine.UI;
using Senapp.Engine.PlayerInput;

namespace Senapp.Engine
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

            //TargetRenderFrequency = 60;
            //TargetUpdateFrequency = 30;
        }
        private MasterRenderer renderer = new MasterRenderer();
        public GameObject sunLight = new GameObject();
        public GameObject mainCamera = new GameObject();

        private int SortByDistanceToCamera(GameObject a, GameObject b)
        {
            return Vector3.Distance(a.transform.position, mainCamera.transform.position).CompareTo(Vector3.Distance(b.transform.position, mainCamera.transform.position));
        }
        private int SortByZ(GameObject a, GameObject b)
        {
            return a.transform.position.Z.CompareTo(b.transform.position.Z);
        }

        private RaycastTarget currentTarget;
        private RaycastTargetUI currentTargetUI;

        private void RaycastSendingUpdate(MouseMoveEventArgs e)
        {
            var cam = mainCamera.GetComponent<Camera>();
            var sortedObjects = new List<GameObject>(GameObject.GameObjects);
            sortedObjects.Sort(SortByDistanceToCamera);
            foreach (var gameObject in sortedObjects)
            {
                if (gameObject.HasComponent<RaycastTarget>() && gameObject.enabled)
                {
                    var target = gameObject.GetComponent<RaycastTarget>();
                    float dist = Raycast.DistanceFromPoint(new Point(e.X, e.Y), new Vector3(0, 0, 0), gameObject.transform.TransformationMatrix() * cam.GetViewMatrix(), cam.GetProjectionMatrix());

                    if (dist <= target.hitRadius && !target.hovering && currentTarget == null && currentTargetUI == null)
                    {
                        if (target.onEnter != null) target.onEnter();
                        target.hovering = true;
                        currentTarget = target;
                        return;
                    }
                    else if (dist <= target.hitRadius && !target.hovering && currentTarget != null && currentTargetUI == null)
                    {
                        if (Vector3.Distance(currentTarget.gameObject.transform.position, mainCamera.transform.position) > Vector3.Distance(target.gameObject.transform.position, mainCamera.transform.position))
                        {
                            if (currentTarget.onExit != null) currentTarget.onExit();
                            currentTarget.hovering = false;
                            if (target.onEnter != null) target.onEnter();
                            target.hovering = true;
                            currentTarget = target;
                            return;
                        }
                    }
                    else if (dist > target.hitRadius && target.hovering && currentTarget != null && currentTargetUI == null)
                    {
                        if (target.onExit != null) target.onExit();
                        target.hovering = false;
                        currentTarget = null;
                        return;
                    }
                }
            }
        }
        private void RaycastUISendingUpdate(MouseMoveEventArgs e)
        {
            var sortedObjects = new List<GameObject>(GameObject.GameObjects);
            sortedObjects.Sort(SortByZ);
            foreach (var gameObject in sortedObjects)
            {
                if (gameObject.HasComponent<RaycastTargetUI>() && gameObject.enabled)
                {
                    var target = gameObject.GetComponent<RaycastTargetUI>();
                    var inBox = false;

                    if (target.gameObject.HasComponent<Text>())
                    {
                        var text = target.gameObject.GetComponent<Text>();
                        var dimensions = text.gameObject.transform.GetUIDimensionsPixels(true, text);

                        var minX = dimensions.X;
                        var maxX = dimensions.Z;

                        var minY = dimensions.Y;
                        var maxY = dimensions.W;

                        inBox = minX <= e.X && e.X <= maxX && minY <= e.Y && e.Y <= maxY;
                    }
                    else if (target.gameObject.HasComponent<UIElement>())
                    {
                        var element = target.gameObject.GetComponent<UIElement>();
                        var dimensions = element.gameObject.transform.GetUIDimensionsPixels(false);

                        var minX = dimensions.X;
                        var maxX = dimensions.Z;

                        var minY = dimensions.Y;
                        var maxY = dimensions.W;

                        inBox = minX <= e.X && e.X <= maxX && minY <= e.Y && e.Y <= maxY;
                    }

                    if (inBox && !target.hovering && currentTargetUI == null)
                    {
                        if (currentTarget != null)
                        {
                            if (currentTarget.onExit != null) currentTarget.onExit();
                            currentTarget.hovering = false;
                            currentTarget = null;
                        }
                        if (target.onEnter != null) target.onEnter();
                        target.hovering = true;
                        currentTargetUI = target;
                        return;
                    }
                    else if (!inBox && !target.hovering && currentTargetUI != null)
                    {
                        if (currentTargetUI.gameObject.transform.position.Z > target.gameObject.transform.position.Z)
                        {
                            if (currentTargetUI.onExit != null) currentTargetUI.onExit();
                            currentTargetUI.hovering = false;
                            if (target.onEnter != null) target.onEnter();
                            target.hovering = true;
                            currentTargetUI = target;
                            return;
                        }
                    }
                    else if (!inBox && target.hovering && currentTargetUI != null)
                    {
                        if (target.onExit != null) target.onExit();
                        target.hovering = false;
                        currentTargetUI = null;
                        return;
                    }
                }
            }
        }
        private void RaycastClickCheck()
        {
            foreach (var gameObject in GameObject.GameObjects)
            {
                if (gameObject.HasComponent<RaycastTargetUI>() && gameObject.enabled)
                {
                    var target = gameObject.GetComponent<RaycastTargetUI>();

                    if (target.hovering) target.onClick();
                }
                else if (gameObject.HasComponent<RaycastTarget>() && gameObject.enabled)
                {
                    var target = gameObject.GetComponent<RaycastTarget>();

                    if (target.hovering) target.onClick();
                }
            }
        }
        private void PhysicsUpdate()
        {
            foreach (var mesh in BoxCollisionMesh.colliders)
            {
                if (mesh.gameObject.enabled && mesh.gameObject.HasComponent<Rigidbody>())
                {
                    mesh.UpdateBoxTransform();
                    var body = mesh.gameObject.GetComponent<Rigidbody>();
                    body.falling = true;
                    foreach (var col in BoxCollisionMesh.colliders)
                    {
                        if (col.gameObject.enabled && col != mesh)
                        {
                            if (col.CheckCollision(mesh, out Vector3 position, out bool grounded))
                            {
                                mesh.gameObject.transform.position = position;
                                if (grounded) body.falling = false;
                            }
                        }
                    }
                }
                else if (mesh.gameObject.enabled && !mesh.gameObject.isStatic) mesh.UpdateBoxTransform();
            }
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
            mainCamera.AddComponent(new Camera(Width / (float)Height, 60));
            sunLight.AddComponent(new Light(Vector3.One));

            MasterRenderer.Initialize();
            renderer.Initiliaze(mainCamera.GetComponent<Camera>());


            GL.Enable(EnableCap.Texture2D);
            GL.Ortho(0, Width, Height, 0, 0, 1);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            EditorWindow.Init(this);
            OnGameInitialized();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Input.Update();
            ControllerManager.Update();
            OnGameUpdated(new GameUpdatedEventArgs((float)e.Time));
            foreach (var gameObject in GameObject.GameObjects)
            {
                if (!gameObject.enabled) continue;
                foreach (var component in gameObject.componentManager.GetComponents())
                {
                    component.Value.Update(new GameUpdatedEventArgs((float)e.Time));
                }
                
            }
            PhysicsUpdate();
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            RaycastUISendingUpdate(e);
            RaycastSendingUpdate(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            RaycastClickCheck();
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
            FrameRate.Update();
            MasterRenderer.ClearScreen();
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            renderer.Render(sunLight.GetComponent<Light>(), mainCamera.GetComponent<Camera>());
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
