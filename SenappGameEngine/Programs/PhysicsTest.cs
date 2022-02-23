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
using Senapp.Engine.Models;
using Senapp.Engine.Physics;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Raycasts;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;
using Senapp.Engine.Utilities.Testing;

namespace Senapp.Programs
{
    public class PhysicsTest : Game
    {
        public PhysicsTest(GraphicsMode GRAPHICS_MODE) : base(WIDTH, HEIGHT, GRAPHICS_MODE, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameUpdatedEvent += Update;
            Run();
        }

        public static readonly bool START_FULLSCREEN = false;
        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "Test";

        public GameFont font = new();
        private void Initialize(object sender)
        {
            font.LoadFont("opensans");
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
            plane.transform = new Transform(plane, size / 2, 2, size / 2);
            plane.AddComponent(new RigidEntity(plane.transform.GetWorldPosition()));
            MainScene.AddGameObject(plane);

            for (int i = 0; i < 100; i++)
            {
                GameObject obj = new();
                obj.AddComponent(new Entity(Geometries.Cube, ""));
                obj.transform.SetPosition(new Vector3(Randomize.RangeFloat(1, 10), Randomize.RangeFloat(1, 10), Randomize.RangeFloat(1, 10)));
                obj.GetComponent<Entity>().model.shineDamper = 0.1f;
                obj.GetComponent<Entity>().model.reflectivity = 0.1f;
                obj.GetComponent<Entity>().model.luminosity = 0.8f;
                obj.AddComponent(new RigidEntity(obj.transform.GetWorldPosition()));
                obj.AddComponent(new RaycastTarget(1, onEnter: () =>
                {
                    obj.colour = Color.Red;
                    lockToMouseObject = obj;
                }, onExit: () =>
                {
                    obj.colour = Color.White;
                    if (lockToMouseObject == obj)
                    {
                        lockToMouseObject = null;
                    }
                }));
                MainScene.AddGameObject(obj);
            }

            SunLight.gameObject.transform = new Transform(SunLight.gameObject, 0, 25, 0, 0, 0, 0, 2f, 2f, 2f);
            SunLight.gameObject.AddComponent(new Entity(Geometries.Sphere));
            SunLight.gameObject.GetComponent<Entity>().model.luminosity = 1;

            MainCamera.gameObject.transform = new Transform(MainCamera.gameObject, 0, 4, 10);

            ProfilerScreen = new GameObjectUI()
                .WithParent(MainScene)
                .WithName("profiler")
                .WithColour(Color.Black)
                .WithScale(new Vector3(0.6f, 0.15f, 0.5f))
                .AddComponent(new Sprite());

            ProfilerScreen.UIConstriant = UIPosition.Left;

            text1 = new GameObjectUI()
                .WithParent(ProfilerScreen.gameObject)
                .WithName("text1")
                .WithPosition(new Vector3(22, 3f, 0))
                .AddComponent(new Text("FPS", font, 8));

            text2 = new GameObjectUI()
                .WithParent(ProfilerScreen.gameObject)
                .WithName("text2")
                .WithPosition(new Vector3(22, -3f, 0))
                .AddComponent(new Text("Memory", font, 8));
        }

        Text text1;
        Text text2;
        Sprite ProfilerScreen;

        GameObject lockToMouseObject;

        private void DebugScreenTexts()
        {
            text1.UpdateText("FPS: " + FrameRate.FPS.ToString());
            text2.UpdateText("GameObjects: " + GetAllGameObjects().Count);
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;

            DebugScreenTexts();
            if (lockToMouseObject != null)
            {
                if (Input.GetMouseButtonDown(MouseButton.Right))
                {
                    var component = lockToMouseObject.GetComponent<RaycastTarget>();
                    component.focused = false;
                    component.onLoseFocus();
                    lockToMouseObject = null;
                    return;
                }

                if (!Input.GetKey(Key.R))
                {
                    var mult = 0.5f;
                    if (Input.GetKey(Key.Up))
                        lockToMouseObject.transform.Translate(lockToMouseObject.transform.Up * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Down))
                        lockToMouseObject.transform.Translate(-lockToMouseObject.transform.Up * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Left))
                        lockToMouseObject.transform.Translate(-lockToMouseObject.transform.Right * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Right))
                        lockToMouseObject.transform.Translate(lockToMouseObject.transform.Right * args.DeltaTime * mult);
                    if (Input.GetKey(Key.ControlRight))
                        lockToMouseObject.transform.Translate(-lockToMouseObject.transform.Front * args.DeltaTime * mult);
                    if (Input.GetKey(Key.ShiftRight))
                        lockToMouseObject.transform.Translate(lockToMouseObject.transform.Front * args.DeltaTime * mult);
                }
                else
                {
                    var mult = 20;
                    if (Input.GetKey(Key.Up))
                        lockToMouseObject.transform.Rotate(lockToMouseObject.transform.Up * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Down))
                        lockToMouseObject.transform.Rotate(-lockToMouseObject.transform.Up * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Left))
                        lockToMouseObject.transform.Rotate(-lockToMouseObject.transform.Right * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Right))
                        lockToMouseObject.transform.Rotate(lockToMouseObject.transform.Right * args.DeltaTime * mult);
                    if (Input.GetKey(Key.ControlRight))
                        lockToMouseObject.transform.Rotate(-lockToMouseObject.transform.Front * args.DeltaTime * mult);
                    if (Input.GetKey(Key.ShiftRight))
                        lockToMouseObject.transform.Rotate(lockToMouseObject.transform.Front * args.DeltaTime * mult);
                }
            }

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
            if (Input.GetKeyDown(Key.V) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadRight))
            {
                if (VSync == VSyncMode.Off) VSync = VSyncMode.On;
                else VSync = VSyncMode.Off;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            MainCamera.SetFov(MainCamera.Fov - e.DeltaPrecise);
            base.OnMouseWheel(e);
        }
    }
}
