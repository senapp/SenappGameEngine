using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using Senapp.Engine.Core;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Core.Transforms;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.Loaders.Models;
using Senapp.Engine.Physics;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Raycasts;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;
using Senapp.Engine.Utilities.Testing;
using Senapp.Engine.Controllers;
using Senapp.Engine.Models;

namespace Senapp.Programs
{
    public class TestGame : Game
    {
        public TestGame(GraphicsMode GRAPHICS_MODE) : base(WIDTH, HEIGHT, GRAPHICS_MODE, TITLE)
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

        public GameFont font = new();
        private void Initialize(object sender)
        {
            font.LoadFont("arial");
            Icon = Resources.GetIcon("new_icon");

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
            GameObject plane = new();
            int size = 100;
            plane.AddComponent(new Entity(RawModel.GenerateTerrain(size), ""));
            plane.isStatic = true;
            plane.transform = new Transform(plane, size / 2, 0, size / 2);
            plane.AddComponent(new RigidEntity(plane.transform.GetWorldPosition()));
            MainScene.AddGameObject(plane);

            target.AddComponent(new Entity("alduin", ModelTypes.OBJ));
            target.transform.SetScale(new Vector3(0.02f, 0.02f, 0.02f));
            target.transform.SetPosition(new Vector3(-10, 5, 0));
            target.AddComponent(new RaycastTarget(10, null, () => { Targeting = !Targeting; player.transform.SetRotation(new Vector3(0, player.transform.LookAt(target.transform.GetWorldPosition()).Y - 180, 0)); },null));
            MainScene.AddGameObject(target);

            for (int i = 0; i < 200; i++)
            {
                GameObject obj = new();
                obj.AddComponent(new Entity(Geometries.Sphere, ""));
                obj.colour = new Vector3(Randomize.RangeFloat(0f, 1f), Randomize.RangeFloat(0f, 1f), Randomize.RangeFloat(0f, 1f)).ToColour();
                obj.transform.SetPosition(new Vector3(Randomize.RangeFloat(-50f, 50f), Randomize.RangeFloat(0.5f, 50f), Randomize.RangeFloat(-50f, 50f)));
                obj.transform.SetScale(new Vector3(Randomize.RangeFloat(0.5f, 2f)));
                obj.GetComponent<Entity>().model.reflectivity = 0.5f;
                MainScene.AddGameObject(obj);
            }

            player.AddComponent(new Entity("witcher", ModelTypes.OBJ));
            player.transform.SetScale(new Vector3(2, 2, 2));
            MainScene.AddGameObject(player);

            SunLight.gameObject.transform = new Transform(SunLight.gameObject, - 200, 2000, -200, 0, 0, 0, 2f, 2f, 2f);
            SunLight.gameObject.AddComponent(new Entity(Geometries.Sphere));
            SunLight.gameObject.GetComponent<Entity>().model.luminosity = 1;

            MainCamera.gameObject.transform = new Transform(MainCamera.gameObject, 0, 4, 10);

            var healthbar = new GameObjectUI()
                .WithParent(MainScene)
                .WithScale(new Vector3(0.8f, 100f / 256f * 0.8f, 0.5f))
                .WithPosition(new Vector3(5, -10, 0))
                .AddComponent(new Sprite("healthbar"));

            healthbar.UIConstriant = UIPosition.TopLeft;

            var minimap = new GameObjectUI()
                .WithParent(MainScene)
                .WithScale(new Vector3(0.8f, 128f / 256f * 0.8f, 0.5f))
                .WithPosition(new Vector3(-16, -10, 0))
                .AddComponent(new Sprite("minimap"));

            minimap.UIConstriant = UIPosition.TopRight;

            questContainer = new GameObjectUI()
                .WithName("text12")
                .WithParent(MainScene)
                .WithPosition(new Vector3(65, 45, 0))
                .AddComponent(new Sprite());

            questContainer.gameObject.visible = false;
            questContainer.UIConstriant = UIPosition.CenterRight;

            var questText = new GameObjectUI()
                .WithParent(questContainer.gameObject)
                .WithPosition(new Vector3(0, 0, 0))
                .WithColour(new Vector3(216f / 255f, 150f / 255f, 63f / 255f).ToColour())
                .AddComponent(new Text("Ladies of the Wood", font, 6, dock: Dock.Center));

            var questDes1 = new GameObjectUI()
                .WithParent(questContainer.gameObject)
                .WithPosition(new Vector3(0, -7, 0))
                .WithColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f).ToColour())
                .AddComponent(new Text("Strange women lying", font, 4, dock: Dock.Center));

            var questDes2 = new GameObjectUI()
                .WithParent(questContainer.gameObject)
                .WithPosition(new Vector3(0, -10, 0))
                .WithColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f).ToColour())
                .AddComponent(new Text("in ponds distributing", font, 4, dock: Dock.Center));

            var questDes3 = new GameObjectUI()
                .WithParent(questContainer.gameObject)
                .WithPosition(new Vector3(0, -13, 0))
                .WithColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f).ToColour())
                .AddComponent(new Text("swords is no basis for", font, 4, dock: Dock.Center));

            var questDes4 = new GameObjectUI()
                .WithParent(questContainer.gameObject)
                .WithColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f).ToColour())
                .WithPosition(new Vector3(0, -16, 0))
                .AddComponent(new Text("a system of government", font, 4, dock: Dock.Center));

            ProfilerScreen = new GameObjectUI()
               .WithParent(MainScene)
               .WithName("profiler")
               .WithScale(new Vector3(0.6f, 0.15f, 0.5f))
               .WithColour(Color.Black)
               .AddComponent(new Sprite());

            ProfilerScreen.UIConstriant = UIPosition.Left;

            text1 = new GameObjectUI()
                .WithName("text1")
                .WithParent(ProfilerScreen.gameObject)
                .WithPosition(new Vector3(22, 3f, 0))
                .AddComponent(new Text("FPS", font, 8));

            text2 = new GameObjectUI()
                .WithParent(ProfilerScreen.gameObject)
                .WithName("text2")
                .WithPosition(new Vector3(22, -3f, 0))
                .AddComponent(new Text("Memory", font, 8));
        }

        readonly GameObject player = new();
        readonly GameObject target = new();

        Text text1;
        Text text2;
        Sprite ProfilerScreen;
        Sprite questContainer;

        private bool Gaming;
        private bool Targeting;

        private void DebugScreenTexts()
        {
            text1.UpdateText("FPS: " + FrameRate.FPS.ToString());
            text2.UpdateText("GameObjects: " + Game.Instance.GetAllGameObjects().Count);
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;
            DebugScreenTexts();

            if (Input.GetKeyDown(Key.Escape) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.Back))
            {
                Exit();
            }
            if (Input.GetKeyDown(Key.Z) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadUp))
            {
                WireFrame.Enable(!WireFrame.IsEnabled());
            }
            if (Input.GetKeyDown(Key.Q) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadLeft))
            {
                ProfilerScreen.gameObject.enabled = !ProfilerScreen.gameObject.enabled;
            }
            if (Input.GetKeyDown(Key.G) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.Start))
            {
                Gaming = !Gaming;
            }
            if (Input.GetKeyDown(Key.V) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadRight))
            {
                if (VSync == VSyncMode.Off) VSync = VSyncMode.On;
                else VSync = VSyncMode.Off;
            }

            if (Gaming)
            {
                const float movementSpeed = 10.0f;
                var movement = Vector3.Zero;

                if (ControllerManager.ControllerExists(0)) movement.Z = ControllerManager.GetController(0).GetAxis(Axis.VerticalLeft);
                if (Input.GetKey(Key.W)) movement.Z += 1;
                if (Input.GetKey(Key.S)) movement.Z -= 1;

                if (ControllerManager.ControllerExists(0)) movement.X = -ControllerManager.GetController(0).GetAxis(Axis.HorizontalLeft);
                if (Input.GetKey(Key.A)) movement.X += 1;
                if (Input.GetKey(Key.D)) movement.X -= 1;

                if (MainCamera.gameObject.GetComponent<CameraController>().CameraFollowMouse)
                {
                    Vector2 delta = Input.GetMouseDelta();

                    if (ControllerManager.ControllerExists(0))
                    {
                        var newDelta = ControllerManager.GetController(0).GetAxis(2, 3);
                        if (Math.Abs(newDelta.X) > 0)
                        {
                            delta = newDelta;
                        }
                    }

                    if (Input.CursorLocked) Input.SetMousePositionWindowCenter(0, 0);

                     // if (delta.X < 0) MainCamera.gameObject.transform.position -= MainCamera.gameObject.transform.Right * 10 * args.DeltaTime;
                     // if (delta.X > 0) MainCamera.gameObject.transform.position += MainCamera.gameObject.transform.Right * 10 * args.DeltaTime;      
                     // MainCamera.gameObject.transform.LookAt(player.transform.position);
                }
                else
                {
                    if (movement.Z != 0 || movement.X != 0)
                    {
                        var camForward = MainCamera.gameObject.transform.Front;
                        camForward.Y = 0;
                        camForward.Normalize();
                        var camRight = MainCamera.gameObject.transform.Right;
                        camRight.Y = 0;
                        camRight.Normalize();

                        if (!Targeting)
                        {
                            var direction = player.transform.GetWorldPosition() + (camForward * -movement.Z + camRight * movement.X).Normalized();
                            player.transform.RotateTowardsTarget(direction, args.DeltaTime * 5, 270);
                        }
                        else
                        {
                            player.transform.RotateTowardsTarget(target.transform.GetWorldPosition(), 1, 90);
                        }


                        var movementVector = new Vector3(((camForward * -movement.Z + camRight * movement.X).Normalized() * movementSpeed * args.DeltaTime).X, 0, ((camForward * -movement.Z + camRight * movement.X).Normalized() * movementSpeed * args.DeltaTime).Z);

                        player.transform.Translate(-movementVector);
                    }

                    var pos = player.transform.GetWorldPosition();
                    MainCamera.gameObject.transform.SetRotation(new Vector3(-15f, -180, MainCamera.gameObject.transform.GetWorldRotation().Z));
                    MainCamera.gameObject.transform.SetPosition(new Vector3(pos.X, pos.Y + 6, pos.Z - 7));
                }
            }
        }
        private void Render(object sender, GameRenderedEventArgs e) { }
        private void Resized(object sender) { }
        private void Close(object sender) { }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            MainCamera.SetFov(MainCamera.Fov - e.DeltaPrecise);
            base.OnMouseWheel(e);
        }
    }
}
